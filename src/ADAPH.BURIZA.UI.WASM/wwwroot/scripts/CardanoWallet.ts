import {generateMnemonic, validateMnemonic, mnemonicToEntropy} from "bip39";
import {harden} from "./CardanoHelper";
import CardanoLoader from "./CardanoLoader";

const PURPOSE = 1852; 
const COIN_TYPE = 1815; // CARDANO - refer SLIP
export class CardanoWallet {
	
	GenerateSeedPhrase = () : string => {
		return generateMnemonic(256);
	};

	ValidateSeedPhrase = (seedPhrase: string) : boolean => {
		return validateMnemonic(seedPhrase);
	};
	
	GenerateWalletRootKey = async (seedPhrase: string, passPhrase: string) => {
		if(window.CardanoInterop.SerializationLib != null) {
			// Check if seed phrase is valid
			if(this.ValidateSeedPhrase(seedPhrase)) {
				
				// entropy for root key generation
				const entropy = mnemonicToEntropy(seedPhrase);

				let rootKey = window.CardanoInterop.SerializationLib.Bip32PrivateKey.from_bip39_entropy(
					Buffer.from(entropy, "hex"),
					Buffer.from("")
				);
				
				// TODO: encrypt rootkey with Passphrase
				localStorage.setItem("rootKey", rootKey.to_bech32());
			}
		}
	}
	
	GenerateWalletAddress = async (rootKeyString: string, passPhrase: string, isMainnet: boolean) => {
		
		let networkId = window.CardanoInterop.SerializationLib.NetworkInfo.testnet().network_id();
		
		if(isMainnet) {
			networkId = window.CardanoInterop.SerializationLib.NetworkInfo.mainnet().network_id();
		}
		
		// TODO: decode rootKeyString with passphrase
		let rootKey = window.CardanoInterop.SerializationLib.Bip32PrivateKey.from_bech32(rootKeyString);

		let accountKey = rootKey
			.derive(harden(1852)) // purpose
			.derive(harden(1815)) // coin type
			.derive(harden(0)); // account #0
		
		let utxoPubKey = accountKey
			.derive(0) // external
			.derive(0)
			.to_public();

		let stakeKey = accountKey
			.derive(2) // chimeric
			.derive(0)
			.to_public();
		
		let baseAddress = window.CardanoInterop.SerializationLib.BaseAddress.new(
			networkId,
			window.CardanoInterop.SerializationLib.StakeCredential.from_keyhash(utxoPubKey.to_raw_key().hash()),
			window.CardanoInterop.SerializationLib.StakeCredential.from_keyhash(stakeKey.to_raw_key().hash())
		)
	}
	
}

const CardanoWalletApi = new CardanoWallet();
export {CardanoWalletApi}