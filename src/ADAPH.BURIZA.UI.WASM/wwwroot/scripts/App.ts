import CardanoLoader from "./CardanoLoader";
import {CardanoWalletApi} from "./CardanoWallet";
import {CONFIG} from "./Config";

window.CardanoInterop = {
    Wallet: CardanoWalletApi,
    Config: CONFIG
};

document.onreadystatechange = async () => {
    if (document.readyState === "complete") {
        window.CardanoInterop.SerializationLib = await CardanoLoader.LoadAsync();
    }
}