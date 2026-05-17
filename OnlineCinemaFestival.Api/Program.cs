using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Hubs;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.AcessosFolder;
using OnlineCinemaFestival.Api.Services.Catalogo;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddSignalR();

if (builder.Environment.IsDevelopment())
{
    var dataProtectionKeysPath = Path.Combine(
        Path.GetTempPath(),
        "OnlineCinemaFestival.Api",
        "DataProtectionKeys"
    );

    Directory.CreateDirectory(dataProtectionKeysPath);

    var dataProtectionBuilder = builder
        .Services.AddDataProtection()
        .SetApplicationName("OnlineCinemaFestival.Api")
        .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

    if (OperatingSystem.IsWindows())
        dataProtectionBuilder.ProtectKeysWithDpapi();
}

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
                .AllowAnyHeader()
                .AllowCredentials();
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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                    context.Token = accessToken;

                return Task.CompletedTask;
            },
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
builder.Services.AddScoped<IValidadorFinalizacaoCompra, ValidadorFinalizacaoCompra>();

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

builder.Services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IPagamentoService, PagamentoSimuladoService>();
builder.Services.AddScoped<IAcessoUtilizadorService, AcessoUtilizadorService>();

builder.Services.AddScoped<ISessaoRepository, SessaoRepository>();
builder.Services.AddScoped<ISessaoService, SessaoService>();
builder.Services.AddScoped<IMensagemChatSessaoRepository, MensagemChatSessaoRepository>();
builder.Services.AddScoped<IChatSessaoService, ChatSessaoService>();

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

builder.Services.AddScoped<IPrecoStrategy, BilheteIndividualPrecoStrategy>();
builder.Services.AddScoped<IPrecoStrategy, PasseFestivalPrecoStrategy>();
builder.Services.AddScoped<IRewardsRepository, RewardsRepository>();
builder.Services.AddScoped<IRewardTransacaoRepository, RewardTransacaoRepository>();
builder.Services.AddScoped<ICompraObserver, AcessoObserver>();
builder.Services.AddScoped<ICompraObserver, RewardsObserver>();
builder.Services.AddScoped<IRewardsQueryService, RewardsQueryService>();
builder.Services.AddScoped<ICompraItemValidator, BilheteSessaoCompraItemValidator>();
builder.Services.AddScoped<ICompraItemValidator, AluguerDigitalCompraItemValidator>();
builder.Services.AddScoped<ICompraValidator, CompraValidator>();
builder.Services.AddScoped<IAcessoFactory, AcessoFactory>();
builder.Services.AddScoped<ICompraHistoricoService, CompraHistoricoService>();
builder.Services.AddScoped<ICinemaFacade, CinemaFacade>();
builder.Services.AddScoped<IPremioFestivalService, PremioFestivalService>();

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
builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();
builder.Services.AddScoped<IPerfilUtilizadorService, PerfilUtilizadorService>();

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

if (Directory.Exists(app.Environment.WebRootPath))
    app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SessaoChatHub>("/hubs/sessoes-chat");
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    var passwordHashingStrategy =
        scope.ServiceProvider.GetRequiredService<IPasswordHashingStrategy>();

    await DbSeeder.SeedAsync(db, passwordHashingStrategy);
}
app.Run();
