using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartHub.Consumer;
using SmartHub.Website;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

//const string baseAddress = "http://192.168.1.9:5010/";
//const string baseAddress = "http://192.168.1.76:5000/";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(sp.GetService<IConfiguration>()["apiUrl"]) });
builder.Services.AddScoped<RaspberryClient>();

await builder.Build().RunAsync();

