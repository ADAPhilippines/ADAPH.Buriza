
using ADAPH.BURIZA.UI.WASM.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;

namespace ADAPH.BURIZA.UI.WASM.Pages;
public partial class Index
{
    [Inject] private CardanoWalletInteropService? CardanoWalletInteropService { get; set; }
    [Inject] private ISnackbar? Snackbar { get; set; }
    private bool IsSeedPhraseShown { get; set; }
    private bool IsValidateSeedPhraseShown { get; set; }
    private string[]? GeneratedMnemonicWords { get; set; }
    private List<string> ValidationSeedPhrase { get; set; } = new();
    
    private const int MAX_SEED_PHRASE_COUNT = 24;
    private string ValidationSeedPhraseRawText { get; set; } = string.Empty;
    private bool hasWalletId { get; set; }
    private string ReceivingAddress { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            if (Snackbar is not null)
            {
                Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomCenter;
                Snackbar.Configuration.MaxDisplayedSnackbars = 1;
                Snackbar.Configuration.NewestOnTop = true;
                Snackbar.Configuration.ShowTransitionDuration = 300;
                Snackbar.Configuration.HideTransitionDuration = 300;
                Snackbar.Configuration.VisibleStateDuration = 2000;
            }
            Snackbar?.Add("Retrieving Wallet Data", Severity.Info);
            await ProcessWalletRetrievalAsync();
            await InvokeAsync(StateHasChanged);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async void OnCreateButtonClick(MouseEventArgs args)
    {
        if(CardanoWalletInteropService is null) throw new NullReferenceException("CardanoWalletInteropService is null");
        
        var mnemonicWordsRaw = await CardanoWalletInteropService.GenerateSeedPhraseAsync();
        GeneratedMnemonicWords = mnemonicWordsRaw.Split(" ").ToArray();
        IsSeedPhraseShown = true;
        
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnRestoreButtonClick(MouseEventArgs args)
    {
        ValidationSeedPhrase.Clear();
        IsSeedPhraseShown = false;
        IsValidateSeedPhraseShown = true;
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnCancelButtonClick(MouseEventArgs args)
    {
        
        IsSeedPhraseShown = false;
        IsValidateSeedPhraseShown = false;
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnValidationTextFieldKeyUp(KeyboardEventArgs args)
    {
        if (args.Code is "Enter" or "Space")
        {   
            var value = ValidationSeedPhraseRawText.Split(" ");
            if (value.Length > 0)
            {
                foreach (var word in value)
                {
                    if (!string.IsNullOrEmpty(word.Trim()))
                    {
                        ValidationSeedPhrase.Add(word.Trim());
                    }
                }
                ValidationSeedPhraseRawText = string.Empty;
            }
        }
        else
        {
            // maybe pasted value
            var value = ValidationSeedPhraseRawText.Split(" ");
            if (value.Length > 1)
            {
                foreach (var word in value)
                {
                    if (!string.IsNullOrEmpty(word.Trim()))
                    {
                        if(ValidationSeedPhrase.Count < MAX_SEED_PHRASE_COUNT)
                            ValidationSeedPhrase.Add(word.Trim());
                    }
                }
                ValidationSeedPhraseRawText = string.Empty;
            }
        }
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnClearButtonClick(MouseEventArgs args)
    {
        ValidationSeedPhrase.Clear();
        ValidationSeedPhraseRawText = string.Empty;
        await InvokeAsync(StateHasChanged);
    }
    
    private async void OnValidateButtonClick(MouseEventArgs args)
    {
        if(CardanoWalletInteropService is null) throw new NullReferenceException("CardanoWalletInteropService is null");
        var rawSeedPhrase = string.Join(" ", ValidationSeedPhrase);
        var isSeedPhraseValid = await CardanoWalletInteropService.ValidateSeedPhraseAsync(rawSeedPhrase);

        if (isSeedPhraseValid is true)
        {
            Snackbar?.Add("Seed phrase is valid. Restoring Wallet...", Severity.Success);
            await CardanoWalletInteropService.GenerateWalletAccountAsync(rawSeedPhrase, 0);
            await ProcessWalletRetrievalAsync();
        }
        else
        {
            Snackbar?.Add("Seed phrase is invalid. Please re-enter seed phrase..", Severity.Error);
            ValidationSeedPhrase.Clear();
            ValidationSeedPhraseRawText = string.Empty;
            ReceivingAddress = string.Empty;
        }
        await InvokeAsync(StateHasChanged);
    }
    
    private bool ShouldDisableValidationTextField()
    {
        return ValidationSeedPhrase.Count >= 24;
    }
    
    private async void OnRemoveButtonClick(MouseEventArgs args)
    {
        if(CardanoWalletInteropService is null) throw new NullReferenceException("CardanoWalletInteropService is null");
        await CardanoWalletInteropService.RemoveWalletIdAsync();
        ValidationSeedPhrase.Clear();
        IsSeedPhraseShown = false;
        IsValidateSeedPhraseShown = false;
        hasWalletId = false;
        ReceivingAddress = string.Empty;
        await InvokeAsync(StateHasChanged);
    }

    private async Task ProcessWalletRetrievalAsync()
    {
        await Task.Delay(1000);
        if(CardanoWalletInteropService is null) throw new NullReferenceException("CardanoWalletInteropService is null");
        hasWalletId = (await CardanoWalletInteropService.GetWalletIdAsync()) is not null;
        if (hasWalletId)
        {
            ReceivingAddress = await CardanoWalletInteropService.GetWalletReceivingAddressAsync(0);
            Snackbar?.Add("Wallet restored!", Severity.Success);
        }
        else
        {
            ReceivingAddress = string.Empty;
        }
    }
}