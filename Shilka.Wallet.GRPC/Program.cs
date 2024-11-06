using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Shilka.Wallet.GRPC.Services;
using Shilka.Wallet.Persistence;
using Shilka.Wallet.Persistence.Interfaces;
using Shilka.Wallet.Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");

builder.Services.AddGrpc();

var connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPass}";
builder.Services.AddDbContext<ShilkaWalletDbContext>(options =>
	options.UseNpgsql(connectionString));

builder.Services.AddScoped<IFreakWalletRepository, FreakWalletRepository>();

var app = builder.Build();

app.MapGrpcService<FreakWalletService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();