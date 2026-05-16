using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using OnlineCinemaFestival.Client;
using OnlineCinemaFestival.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

const string ApiBaseUrl = "http://localhost:5152/";

builder.Services.AddScoped<ArmazenamentoToken>();
builder.Services.AddScoped<ManipuladorTokenHttp>();
builder.Services.AddScoped<EstadoAutenticacaoCustomizado>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<EstadoAutenticacaoCustomizado>()
);
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

builder
    .Services.AddHttpClient(
        "Api",
        cliente =>
        {
            cliente.BaseAddress = new Uri(ApiBaseUrl);
        }
    )
    .AddHttpMessageHandler<ManipuladorTokenHttp>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("Api"));

builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FilmeService>();
builder.Services.AddScoped<TmdbService>();
builder.Services.AddScoped<FestivalService>();
builder.Services.AddScoped<SessaoService>();
builder.Services.AddScoped<AcessoService>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<CheckoutService>();
builder.Services.AddScoped<CompraService>();
builder.Services.AddScoped<VisualizacaoService>();
builder.Services.AddScoped<ReviewService>();
builder.Services.AddScoped<PerfilService>();
builder.Services.AddScoped<ListaService>();

builder.Services.AddMudServices();

await builder.Build().RunAsync();
