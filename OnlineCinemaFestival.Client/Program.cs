using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using OnlineCinemaFestival.Client;
using OnlineCinemaFestival.Client.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder
    .Services.AddClientAuthentication()
    .AddClientInfrastructure(builder.Configuration)
    .AddClientApplicationServices();

await builder.Build().RunAsync();
