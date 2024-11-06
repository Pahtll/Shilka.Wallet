using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Shilka.Wallet.GRPC.Services;
using Shilka.Wallet.Persistence;
using Shilka.Wallet.Persistence.Interfaces;
using Shilka.Wallet.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Load environment variables from .env file
Env.Load();

// Access environment variables
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");

// Add services to the container.
builder.Services.AddGrpc();

// Configure EF Core to use PostgreSQL
var connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPass}";
builder.Services.AddDbContext<ShilkaWalletDbContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddScoped<IFreakWalletRepository, FreakWalletRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<FreakWalletService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();