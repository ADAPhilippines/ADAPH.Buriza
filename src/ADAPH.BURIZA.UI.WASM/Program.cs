using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ADAPH.BURIZA.UI.WASM;
using ADAPH.BURIZA.UI.WASM.Services;
using Blazored.LocalStorage;
using Blockfrost.Api.Extensions;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<CardanoWalletInteropService>();

var http = new HttpClient()
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
};

if (builder.HostEnvironment.Environment == "Development")
{
    using var response = await http.GetAsync($"appsettings.{builder.HostEnvironment.Environment.ToLower()}.json");
    await using var stream = await response.Content.ReadAsStreamAsync();
    builder.Configuration.AddJsonStream(stream);
}

builder.Services.AddBlockfrost(builder.Configuration["CardanoNetwork"], builder.Configuration["BlockfrostProjectId"]);
builder.Services.AddScoped(sp => http);

await builder.Build().RunAsync();
