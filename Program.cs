using HedgePair.API.Data;
using HedgePair.API.Interfaces;
using HedgePair.API.Repositories;
using HedgePair.API.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ───────────────── CONFIGURATION ─────────────────

// ✅ Load Key Vault (only in Azure / Production)
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVaultUri"];

    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(
            new Uri(keyVaultUri),
            new DefaultAzureCredential());
    }
}

// ───────────────── SERVICES ─────────────────

// ✅ Database (from Azure App Service / Key Vault)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null)));

// ✅ Repositories
builder.Services.AddScoped<IFinancialInstrumentRepository, FinancialInstrumentRepository>();
