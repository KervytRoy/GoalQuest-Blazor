using GoalQuest;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Registra HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Registra ApiClient
builder.Services.AddScoped(sp => new ApiClient.ApiClient("https://webapigoalquest.azurewebsites.net/", sp.GetRequiredService<HttpClient>()));

builder.Services.AddMudServices();
await builder.Build().RunAsync();
