using Microsoft.AspNetCore.Builder; 
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OnlineCinemaFestival.Api.Autorizacao;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;
using OnlineCinemaFestival.Api.Services.Acesso;
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

builder.Services.AddScoped<IFestivalFilmeRepository, FestivalFilmeRepository>();
builder.Services.AddScoped<IFestivalFilmeService, FestivalFilmeService>();

builder.Services.AddScoped<ISessaoRepository, SessaoRepository>();
builder.Services.AddScoped<ISessaoService, SessaoService>();

builder.Services.AddScoped<ICatalogoService, CatalogoService>();

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OnlineCinemaFestival API",
        Version = "v1",
    });

    const string esquemaId = "Bearer";

    options.AddSecurityDefinition(esquemaId, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Cola apenas o token JWT (sem o prefixo 'Bearer'). Obténs o token via POST /api/auth/login.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    });

    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference(esquemaId, doc), new List<string>() },
    });
});

// Parte dos Comentarios
builder.Services.AddScoped<IComentarioRepository, ComentarioRepository>();
builder.Services.AddScoped<IComentarioService, ComentarioService>();

// Parte das Comunidades
builder.Services.AddScoped<IComunidadeRepository, ComunidadeRepository>();
builder.Services.AddScoped<IComunidadeService, ComunidadeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("BlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await DbSeeder.SeedAsync(db);
}
app.Run();
