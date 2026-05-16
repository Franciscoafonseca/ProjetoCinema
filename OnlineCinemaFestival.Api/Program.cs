using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.AcessosFolder;
using OnlineCinemaFestival.Api.Services.Catalogo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=festival.db")
);

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "BlazorClient",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:5002",
                    "https://localhost:7002",
                    "http://localhost:5000",
                    "https://localhost:5001",
                    "http://localhost:5257",
                    "https://localhost:7049"
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

// JWT
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT Key não configurada no appsettings.json.");
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),

            RoleClaimType = ClaimTypes.Role,
            NameClaimType = ClaimTypes.NameIdentifier,
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        NomesPoliticas.ApenasAdministrador,
        policy => policy.RequireRole(NomesPapeis.Administrador)
    );

    options.AddPolicy(
        NomesPoliticas.UtilizadorAutenticado,
        policy => policy.RequireAuthenticatedUser()
    );
});

// Serviços já existentes
builder.Services.AddScoped<IFilmeRepository, FilmeRepository>();
builder.Services.AddScoped<IFilmeService, FilmeService>();

builder.Services.AddScoped<IFestivalRepository, FestivalRepository>();
builder.Services.AddScoped<IFestivalService, FestivalService>();

builder.Services.AddScoped<IVisualizacaoRepository, VisualizacaoRepository>();
builder.Services.AddScoped<IValidacaoAcessoService, ValidacaoAcessoService>();
builder.Services.AddScoped<
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoBilheteSessaoStrategy
>();
builder.Services.AddScoped<
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseDiarioStrategy
>();
builder.Services.AddScoped<
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseCompletoStrategy
>();
builder.Services.AddScoped<
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
    OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoAluguerDigitalStrategy
>();
builder.Services.AddScoped<IVisualizacaoService, VisualizacaoService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUtilizadorAtualService, UtilizadorAtualService>();

builder.Services.AddScoped<IGeradorReferenciaCompra, GeradorReferenciaCompra>();
builder.Services.AddScoped<IValidadorCheckout, ValidadorCheckout>();

builder.Services.AddScoped<IAcessoUtilizadorFactory, FabricaAcessoUtilizador>();

builder.Services.AddScoped<IGeneroService, GeneroService>();
builder.Services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoBilheteSessao>();
builder.Services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseDiario>();
builder.Services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseCompleto>();
builder.Services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoAluguerDigital>();
builder.Services.AddScoped<IFestivalFilmeRepository, FestivalFilmeRepository>();
builder.Services.AddScoped<IFestivalFilmeService, FestivalFilmeService>();
builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<IAcessoUtilizadorRepository, AcessoUtilizadorRepository>();

builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IPagamentoService, PagamentoSimuladoService>();
builder.Services.AddScoped<IAcessoUtilizadorService, AcessoUtilizadorService>();

builder.Services.AddScoped<ISessaoRepository, SessaoRepository>();
builder.Services.AddScoped<ISessaoService, SessaoService>();

builder.Services.AddScoped<ICatalogoService, CatalogoService>();
builder.Services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
builder.Services.AddScoped<ICarrinhoService, CarrinhoService>();

builder.Services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorTituloStrategy>();
builder.Services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorPopularidadeStrategy>();
builder.Services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorClassificacaoStrategy>();
builder.Services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorDataLancamentoStrategy>();
builder.Services.AddScoped<CatalogoOrdenacaoStrategyFactory>();

builder.Services.AddScoped<IAcessoRepository, AcessoRepository>();
builder.Services.AddScoped<IAcessoService, AcessoService>();

builder.Services.AddScoped<IEstrategiaValidacaoAcesso, BilheteSessaoValidacaoStrategy>();
builder.Services.AddScoped<IEstrategiaValidacaoAcesso, EstrategiaValidacaoPasseDiario>();
builder.Services.AddScoped<IEstrategiaValidacaoAcesso, ValidacaoPasseCompletoStrategy>();
builder.Services.AddScoped<IEstrategiaValidacaoAcesso, AluguerDigitalValidacaoStrategy>();

builder.Services.AddScoped<IValidacaoAcessoStrategyFactory, ValidacaoAcessoStrategyFactory>();
builder.Services.AddHttpClient<ITmdbService, TmdbService>();

// Novos repositories
builder.Services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
builder.Services.AddScoped<IGeneroRepository, GeneroRepository>();

// Novos services de autenticação/perfil
builder.Services.AddScoped<IPasswordHashingStrategy, PasswordHashingStrategy>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();

// Listas pessoais
builder.Services.AddScoped<IListaPessoalRepository, ListaPessoalRepository>();
builder.Services.AddScoped<IListaPessoalService, ListaPessoalService>();

// Parte dos Comentarios
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();

// Parte das Comunidades
builder.Services.AddScoped<IComunidadeRepository, ComunidadeRepository>();
builder.Services.AddScoped<IComunidadeService, ComunidadeService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    const string esquemaAutenticacao = "bearer";

    options.AddSecurityDefinition(
        esquemaAutenticacao,
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Insere apenas o token JWT. Não escrevas Bearer.",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(esquemaAutenticacao, document)] = [],
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("BlazorClient");

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var passwordHashingStrategy =
        scope.ServiceProvider.GetRequiredService<IPasswordHashingStrategy>();

    await DbSeeder.SeedAsync(db, passwordHashingStrategy);
}
app.Run();
