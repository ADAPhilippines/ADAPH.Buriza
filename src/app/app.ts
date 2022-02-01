
import { Bip32PrivateKey, BaseAddress, NetworkInfo, StakeCredential } from "@emurgo/cardano-serialization-lib-nodejs";
import * as bip39 from "bip39";

// Index below 0x80000000 results in soft derivation
function harden(num: number): number {
	return 0x80000000 + num;
}

const NETWORK_ID = NetworkInfo.testnet().network_id();
const TEST_MNEMONICS = "crack car town arena aim heart orange wire nasty garden open upon laptop melt shoulder doctor raven goose punch hidden caution simple link child";
//const TEST_MNEMONICS = "mail entire apart ice lottery aspect artwork basic balance bamboo endorse child citizen planet tube radar narrow during penalty sell october retreat exhibit sea";
const TEST_PASSPHRASE = "t3st$!123456"

async function Main() {
	// 256 = 24 words
	var seedPhrase = TEST_MNEMONICS;//bip39.generateMnemonic(256);

	//console.log(seedPhrase.split(" ").length, seedPhrase);

	console.log(seedPhrase);
	var rootKey = Bip32PrivateKey.from_bip39_entropy(
		Buffer.from(seedPhrase, "hex"),
		Buffer.from(TEST_PASSPHRASE)
	);
	
	console.log(rootKey.to_bech32());
	const accountKey = rootKey
		.derive(harden(1852)) // purpose
		.derive(harden(1815)) // coin type
		.derive(harden(0)); // account #0
	
	const utxoPubKey = accountKey
		.derive(0) // external
		.derive(0)
		.to_public();
	
	const stakeKey = accountKey
		.derive(2) // chimeric
		.derive(0)
		.to_public();
	
	const baseAddress = BaseAddress.new(
		NETWORK_ID,
		StakeCredential.from_keyhash(utxoPubKey.to_raw_key().hash()),
		StakeCredential.from_keyhash(stakeKey.to_raw_key().hash())
	)
	console.log("address", baseAddress.to_address().to_bech32());

	utxoPubKey.free();
	baseAddress.free();
	stakeKey.free();
	rootKey.free();
}

(async () => {
	try {
		await Main();
	} catch (err) {
		console.error(err);
	}
})();