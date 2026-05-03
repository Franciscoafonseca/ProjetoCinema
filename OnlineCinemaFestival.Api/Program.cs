using Microsoft.EntityFrameworkCore;
using OnlineCinemaFestival.Api.Data;
using OnlineCinemaFestival.Api.Repositories;
using OnlineCinemaFestival.Api.Services;

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
                    "https://localhost:5001"
                )
                .AllowAnyMethod()
                .AllowAnyHeader();
        }
    );
});

builder.Services.AddScoped<FestivalRepository>();
builder.Services.AddScoped<AcessoRepository>();
builder.Services.AddScoped<RewardsRepository>();
builder.Services.AddScoped<FestivalService>();
builder.Services.AddScoped<CompraService>();
builder.Services.AddScoped<CinemaFacade>();
builder.Services.AddScoped<ICompraObserver, RewardsObserver>();
builder.Services.AddScoped<ICompraObserver, AcessoObserver>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors("BlazorClient");

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();

app.Run();
