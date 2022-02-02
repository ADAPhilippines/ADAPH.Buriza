import CardanoLoader from "./CardanoLoader";
import {CardanoWalletApi} from "./CardanoWallet";

window.CardanoInterop = {
    Wallet: CardanoWalletApi
};

document.onreadystatechange = async () => {
    if (document.readyState === "complete") {
        window.CardanoInterop.SerializationLib = await CardanoLoader.LoadAsync();
    }
}