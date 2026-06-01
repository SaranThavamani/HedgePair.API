using HedgePair.API.Data;
using HedgePair.API.Interfaces;
using HedgePair.API.Repositories;
using HedgePair.API.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ───────────────── CONFIGURATION ─────────────────

// ✅ Load Key Vault (only in Production)
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

// ✅ Database (Azure SQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5), errorNumbersToAdd: null)));

// ✅ Repositories
builder.Services.AddScoped<IFinancialInstrumentRepository, FinancialInstrumentRepository>();
builder.Services.AddScoped<IHedgePairRepository, HedgePairRepository>();

// ✅ Services
builder.Services.AddScoped<IFinancialInstrumentService, FinancialInstrumentService>();
builder.Services.AddScoped<IHedgePairService, HedgePairService>();

// ✅ Controllers
builder.Services.AddControllers();

// ✅ Swagger (IMPORTANT → enable in Azure too)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ✅ TEMP: Disable migration (prevents crash)
// COMMENT THIS for now
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

// ✅ Enable Swagger ALWAYS
app.UseSwagger();
app.UseSwaggerUI();

// ✅ Middleware
app.UseCors("AllowReactApp");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

// ✅ 🔥 CRITICAL FIX → Required for Azure Linux
app.Run("http://0.0.0.0:8080");

// For tests
public partial class Program { }
