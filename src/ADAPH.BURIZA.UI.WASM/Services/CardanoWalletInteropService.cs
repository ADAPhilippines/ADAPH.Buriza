using Microsoft.JSInterop;

namespace ADAPH.BURIZA.UI.WASM.Services;

public class CardanoWalletInteropService
{
    private readonly IJSRuntime? _jsRuntime;
    
    public CardanoWalletInteropService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }
    
    public async Task<string> GenerateSeedPhraseAsync()
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        return await _jsRuntime.InvokeAsync<string>("window.CardanoInterop.Wallet.GenerateSeedPhrase");
    }
    
    public async Task<bool> ValidateSeedPhraseAsync(string rawSeedPhrase)
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        return await _jsRuntime.InvokeAsync<bool>("window.CardanoInterop.Wallet.ValidateSeedPhrase", rawSeedPhrase);
    }
}