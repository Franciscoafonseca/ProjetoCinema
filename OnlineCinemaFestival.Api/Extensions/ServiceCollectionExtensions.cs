using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.AcessosFolder;
using OnlineCinemaFestival.Api.Services.Catalogo;

namespace OnlineCinemaFestival.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApiInfrastructure(
        this IServiceCollection services,
        IWebHostEnvironment environment
    )
    {
        services.AddControllers();
        services.AddMemoryCache();
        services.AddSignalR();

        if (environment.IsDevelopment())
        {
            var dataProtectionKeysPath = Path.Combine(
                Path.GetTempPath(),
                "OnlineCinemaFestival.Api",
                "DataProtectionKeys"
            );

            Directory.CreateDirectory(dataProtectionKeysPath);

            var dataProtectionBuilder = services
                .AddDataProtection()
                .SetApplicationName("OnlineCinemaFestival.Api")
                .PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeysPath));

            if (OperatingSystem.IsWindows())
                dataProtectionBuilder.ProtectKeysWithDpapi();
        }

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=festival.db")
        );

        services.AddCors(options =>
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

        services.AddHttpContextAccessor();
        services.AddEndpointsApiExplorer();

        return services;
    }

    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var jwtKey = jwtSettings["Key"];

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("JWT Key nao configurada no appsettings.json.");

        services
            .AddAuthentication(options =>
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

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
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

        return services;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IFilmeRepository, FilmeRepository>();
        services.AddScoped<IFilmeService, FilmeService>();
        services.AddScoped<IFestivalRepository, FestivalRepository>();
        services.AddScoped<IFestivalService, FestivalService>();
        services.AddScoped<IVisualizacaoRepository, VisualizacaoRepository>();
        services.AddScoped<IValidacaoAcessoService, ValidacaoAcessoService>();
        services.AddScoped<IVisualizacaoService, VisualizacaoService>();
        services.AddScoped<IUtilizadorAtualService, UtilizadorAtualService>();
        services.AddScoped<IGeradorReferenciaCompra, GeradorReferenciaCompra>();
        services.AddScoped<IValidadorFinalizacaoCompra, ValidadorFinalizacaoCompra>();
        services.AddScoped<IAcessoUtilizadorFactory, FabricaAcessoUtilizador>();
        services.AddScoped<IGeneroService, GeneroService>();
        services.AddScoped<IFestivalFilmeRepository, FestivalFilmeRepository>();
        services.AddScoped<IFestivalFilmeService, FestivalFilmeService>();
        services.AddScoped<ICompraRepository, CompraRepository>();
        services.AddScoped<IAcessoUtilizadorRepository, AcessoUtilizadorRepository>();
        services.AddScoped<IFinalizacaoCompraService, FinalizacaoCompraService>();
        services.AddScoped<ICompraService, CompraService>();
        services.AddScoped<IPagamentoService, PagamentoSimuladoService>();
        services.AddScoped<IAcessoUtilizadorService, AcessoUtilizadorService>();
        services.AddScoped<ISessaoRepository, SessaoRepository>();
        services.AddScoped<ISessaoService, SessaoService>();
        services.AddScoped<IMensagemChatSessaoRepository, MensagemChatSessaoRepository>();
        services.AddScoped<IChatSessaoService, ChatSessaoService>();
        services.AddScoped<ICatalogoService, CatalogoService>();
        services.AddScoped<ICarrinhoRepository, CarrinhoRepository>();
        services.AddScoped<ICarrinhoService, CarrinhoService>();
        services.AddScoped<IAcessoRepository, AcessoRepository>();
        services.AddScoped<IAcessoService, AcessoService>();
        services.AddScoped<IAcessoAutomaticoFactory, AcessoAutomaticoFactory>();
        services.AddScoped<IAcessoAutomaticoService, AcessoAutomaticoService>();
        services.AddScoped<IRewardsRepository, RewardsRepository>();
        services.AddScoped<IRewardTransacaoRepository, RewardTransacaoRepository>();
        services.AddScoped<IRewardsQueryService, RewardsQueryService>();
        services.AddScoped<ICompraValidator, CompraValidator>();
        services.AddScoped<IAcessoFactory, AcessoFactory>();
        services.AddScoped<ICompraHistoricoService, CompraHistoricoService>();
        services.AddScoped<ICinemaFacade, CinemaFacade>();
        services.AddScoped<IPremioFestivalRepository, PremioFestivalRepository>();
        services.AddScoped<IPremioFestivalService, PremioFestivalService>();
        services.AddScoped<IValidacaoAcessoStrategyFactory, ValidacaoAcessoStrategyFactory>();
        services.AddScoped<IUtilizadorRepository, UtilizadorRepository>();
        services.AddScoped<IGeneroRepository, GeneroRepository>();
        services.AddScoped<IPasswordHashingStrategy, PasswordHashingStrategy>();
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAutenticacaoService, AutenticacaoService>();
        services.AddScoped<IAutenticacaoExternaService, AutenticacaoExternaService>();
        services.AddScoped<IPerfilFotoUploadService, PerfilFotoUploadService>();
        services.AddScoped<IPerfilUtilizadorService, PerfilUtilizadorService>();
        services.AddScoped<CatalogoTmdbSeedService>();
        services.AddScoped<IListaPessoalRepository, ListaPessoalRepository>();
        services.AddScoped<IListaPessoalService, ListaPessoalService>();
        services.AddScoped<IComentarioRepository, ComentarioRepository>();
        services.AddScoped<IComentarioService, ComentarioService>();
        services.AddScoped<IComunidadeRepository, ComunidadeRepository>();
        services.AddScoped<IComunidadeService, ComunidadeService>();

        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoBilheteSessaoStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseDiarioStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoPasseCompletoStrategy
        >();
        services.AddScoped<
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.IEstrategiaValidacaoAcesso,
            OnlineCinemaFestival.Api.Services.VisualizacaoAcesso.ValidacaoAluguerDigitalStrategy
        >();

        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoBilheteSessao>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseDiario>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoPasseCompleto>();
        services.AddScoped<IEstrategiaCriacaoAcessoUtilizador, EstrategiaCriacaoAluguerDigital>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorTituloStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorPopularidadeStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorClassificacaoStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorDataLancamentoStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategy, OrdenarPorVisualizacoesStrategy>();
        services.AddScoped<ICatalogoOrdenacaoStrategyFactory, CatalogoOrdenacaoStrategyFactory>();
        services.AddScoped<IPrecoStrategy, BilheteIndividualPrecoStrategy>();
        services.AddScoped<IPrecoStrategy, PasseFestivalPrecoStrategy>();
        // Strategy: novos metodos de pagamento entram por DI sem alterar o checkout.
        services.AddScoped<IPagamentoStrategy, PagamentoAprovadoSimuladoStrategy>();
        services.AddScoped<IPagamentoStrategy, PagamentoReferenciaMultibancoStrategy>();
        // Observer: efeitos apos eventos de dominio ficam desacoplados do fluxo principal.
        services.AddScoped<ICompraObserver, AcessoObserver>();
        services.AddScoped<ICompraObserver, RewardsObserver>();
        services.AddScoped<IVisualizacaoObserver, RewardsVisualizacaoObserver>();
        services.AddScoped<IAvaliacaoObserver, RewardsAvaliacaoObserver>();
        // Strategy/OCP: cada tipo de acesso valida as suas proprias regras.
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoBilheteSessaoStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoPasseDiarioStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoPasseCompletoStrategy>();
        services.AddScoped<ICarrinhoAcessoStrategy, CarrinhoAluguerDigitalStrategy>();
        services.AddScoped<ICompraItemValidator, BilheteSessaoCompraItemValidator>();
        services.AddScoped<ICompraItemValidator, AluguerDigitalCompraItemValidator>();
        services.AddScoped<IEstrategiaValidacaoAcesso, BilheteSessaoValidacaoStrategy>();
        services.AddScoped<IEstrategiaValidacaoAcesso, EstrategiaValidacaoPasseDiario>();
        services.AddScoped<IEstrategiaValidacaoAcesso, ValidacaoPasseCompletoStrategy>();
        services.AddScoped<IEstrategiaValidacaoAcesso, AluguerDigitalValidacaoStrategy>();

        services.AddHttpClient<ITmdbApiClient, TmdbApiClient>();
        services.AddScoped<ITmdbService, TmdbService>();

        return services;
    }

    public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
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
                    Description = "Insere apenas o token JWT. Nao escrevas Bearer.",
                }
            );

            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(esquemaAutenticacao, document)] = [],
            });
        });

        return services;
    }
}
