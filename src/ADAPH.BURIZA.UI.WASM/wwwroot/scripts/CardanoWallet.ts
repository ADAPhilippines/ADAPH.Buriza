import {generateMnemonic, validateMnemonic, mnemonicToEntropy} from "bip39";
import {harden} from "./CardanoHelper";
import CardanoProtocolParameters from "./CardanoProtocolParams";
const PURPOSE = 1852; 
const COIN_TYPE = 1815; // CARDANO - refer SLIP

export class CardanoWallet {
	
	GenerateSeedPhrase = () : string => {
		return generateMnemonic(256);
	};

	ValidateSeedPhrase = (seedPhrase: string) : boolean => {
		return validateMnemonic(seedPhrase);
	};

	GenerateWalletAccount = async (seedPhrase: string, passPhrase: string, accountIndex: number) => {
		if(window.CardanoInterop.SerializationLib != null) {
			// Check if seed phrase is valid
			if(this.ValidateSeedPhrase(seedPhrase)) {
				// entropy for root key generation
				const entropy = mnemonicToEntropy(seedPhrase);

				let rootKey = window.CardanoInterop.SerializationLib.Bip32PrivateKey.from_bip39_entropy(
					Buffer.from(entropy, "hex"),
					Buffer.from(passPhrase)
				);
				
				let accountKey = rootKey
					.derive(harden(PURPOSE)) // purpose
					.derive(harden(COIN_TYPE)) // coin type
					.derive(harden(accountIndex)); // account index number
				
				localStorage.setItem("accountId", accountKey.to_public().to_bech32());
				
				// TODO: search about this
				rootKey.free();
			}
		}
	}

	GetWalletReceivingAddress = (addressIndex: number) : string => {
		if (window.CardanoInterop.SerializationLib != null) {
			// Check if seed phrase is valid
			let accountIdBech32 = localStorage.getItem("accountId");
			if (accountIdBech32 != null) {
				
				// TODO: use flag to change network id
				let networkId = window.CardanoInterop.SerializationLib.NetworkInfo.testnet().network_id();
				
				let utxoPubKey = window.CardanoInterop.SerializationLib.Bip32PublicKey
					.from_bech32(accountIdBech32)
					.derive(0) // external
					.derive(addressIndex)
					.to_raw_key();
				
				let stakeKey = window.CardanoInterop.SerializationLib.Bip32PublicKey
					.from_bech32(accountIdBech32)
					.derive(2) // chimeric
					.derive(addressIndex)
					.to_raw_key();
				
				let baseAddress = window.CardanoInterop.SerializationLib.BaseAddress.new(
					networkId,
					window.CardanoInterop.SerializationLib.StakeCredential.from_keyhash(utxoPubKey.hash()),
					window.CardanoInterop.SerializationLib.StakeCredential.from_keyhash(stakeKey.hash())
				);
				return baseAddress.to_address().to_bech32();
			}
		}
	}

	// get protocol params
	GetProtocolProtocolParamsAsync = async (): Promise<CardanoProtocolParameters> => {
		const protocolParamsResult = await fetch(`${window.CardanoInterop.Config.BlockfrostEndpoint}/api/v0/epochs/latest/parameters`, {
			"headers": {
				"project_id": window.CardanoInterop.Config.BlockfrostProjectId
			}
		});
		return await protocolParamsResult.json();
	}

	// Build signed transaction
	BuildTransaction = (protocol: CardanoProtocolParameters)  => {
		let txBuilder = window.CardanoInterop.SerializationLib.TransactionBuilder.new(
			window.CardanoInterop.SerializationLib.LinearFee.new(
				window.CardanoInterop.SerializationLib.BigNum.from_str(protocol.min_fee_a.toString()), 
				window.CardanoInterop.SerializationLib.BigNum.from_str(protocol.min_fee_b.toString())
			),
			window.CardanoInterop.SerializationLib.BigNum.from_str(protocol.min_utxo.toString()),
			window.CardanoInterop.SerializationLib.BigNum.from_str(protocol.pool_deposit.toString()),
			window.CardanoInterop.SerializationLib.BigNum.from_str(protocol.key_deposit.toString()),
			protocol.max_val_size,
			protocol.max_tx_size
		);
		// Get private key - from root? where to store root?
		// Create txInput
		// Create txOutput
		// set witness and sign tx
		// return hex signed tx
	}
}

const CardanoWalletApi = new CardanoWallet();
export {CardanoWalletApi}