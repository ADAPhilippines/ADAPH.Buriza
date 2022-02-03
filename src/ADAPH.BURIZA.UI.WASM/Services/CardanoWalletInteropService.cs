using Blazored.LocalStorage;
using Blockfrost.Api.Services;
using Blockfrost.Api.Services.Extensions;
using CardanoSharp.Wallet.Extensions;
using Microsoft.JSInterop;

namespace ADAPH.BURIZA.UI.WASM.Services;

public class CardanoWalletInteropService
{
    private readonly IJSRuntime? _jsRuntime;
    private readonly ILocalStorageService? _localStorageService;
    private readonly ITransactionsService? _transactionsService;
    private readonly IAddressesService? _addressService;
    public CardanoWalletInteropService(
        IJSRuntime jsRuntime, 
        ILocalStorageService localStorageService, 
        ITransactionsService transactionsService,
        IAddressesService addressService)
    {
        _jsRuntime = jsRuntime;
        _localStorageService = localStorageService;
        _transactionsService = transactionsService;
        _addressService = addressService;
    }
    
    public async Task<string> GenerateSeedPhraseAsync()
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        return await _jsRuntime.InvokeAsync<string>("window.CardanoInterop.Wallet.GenerateSeedPhrase");
    }
    
    public async Task<bool?> ValidateSeedPhraseAsync(string rawSeedPhrase)
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        return await _jsRuntime.InvokeAsync<bool?>("window.CardanoInterop.Wallet.ValidateSeedPhrase", rawSeedPhrase);
    }
    
    public async Task GenerateWalletAccountAsync(string rawSeedPhrase, int accountIndex)
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        await _jsRuntime.InvokeAsync<Task>("window.CardanoInterop.Wallet.GenerateWalletAccount", rawSeedPhrase, string.Empty, accountIndex);
    }
    
    public async Task<string> GetWalletReceivingAddressAsync(int addressIndex)
    {
        if (_jsRuntime is null) throw new NullReferenceException("Javascript Runtime is null");
        return await _jsRuntime.InvokeAsync<string>("window.CardanoInterop.Wallet.GetWalletReceivingAddress", addressIndex, false);
    }
    
    public async Task<string?> GetWalletIdAsync()
    {
        if (_localStorageService is null) throw new NullReferenceException("Local storage service is null");
        return await _localStorageService.GetItemAsync<string?>("accountId");
    }
    
    public async Task RemoveWalletIdAsync()
    {
        if (_localStorageService is null) throw new NullReferenceException("Local storage service is null");
        await _localStorageService.RemoveItemAsync("accountId");
    }
    
    public async Task SubmitTxAsync()
    {
        // TODO: refine and improve
        if (_transactionsService is null) throw new NullReferenceException("Blockfrost transaction service is null");
        var content = "84a30081825820ef0be2a5d721fcf5a8102b33a27e517128606623a20e7e210447748e27b9645800018282583900569f7b43cc1d440f609d9999c650786388b4d73e1176e443d61fea9b11f3726cbf12c559595deb3f6c3c4f5aab98af4254ddc751f44a9a381a1dcd650082583900451dc1c336d7a8303cc3b252f51a80b38c46dbd9334b4a02f468143d470603a99f1cb50a6f724669c6d30a1a030029995ba7bcafa85e89b51a1dcad48b021a00029075a100818258204da52788bca1d795385d8d3be64765024f5aa2383fa64de6b2e01f2fa6f1d59b5840eeb3cc648c47b90e7a471922edd6a043363ba079e8c18adcecfcdde81deabc7383b8a8a3a0609df066adbe72bfc4000208e530ae3f12c93e3c720a654241b800f5f6";
        await _transactionsService.PostTxSubmitAsync(new MemoryStream(content.HexToByteArray()));
    }
}