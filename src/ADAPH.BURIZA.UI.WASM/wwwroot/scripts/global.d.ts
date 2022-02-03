import {CardanoWallet} from "./CardanoWallet";
import {Config} from "./Config";

export {}
declare global {
    interface Window {
        CardanoInterop: {
            Wallet: CardanoWallet
            SerializationLib?: typeof import("@emurgo/cardano-serialization-lib-browser/cardano_serialization_lib"),
            Config: Config
        }
    }
}