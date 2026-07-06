using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.DomainServices;
using SmartPalmPlatform.API.IotDeviceManagement.Application.Internal.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.DomainServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Infrastructure.Persistance.EFC.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Application.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Application.DomainServices;
using SmartPalmPlatform.API.SensorDataProcessing.Application.EventHandlers;
using SmartPalmPlatform.API.SensorDataProcessing.Application.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Repositories;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.CommandServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.DomainServices;
using SmartPalmPlatform.API.SensorDataProcessing.Domain.Services.QueryServices;
using SmartPalmPlatform.API.SensorDataProcessing.Infraestructure.Persistance.EFC;
using SmartPalmPlatform.API.Shared.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

using SmartPalmPlatform.API.AgronomicRecommendation.Application.CommandServices;
using SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
// Npgsql 6+ rejects DateTime with Kind=Local for 'timestamp with time zone'.
// This switch restores the pre-v6 behavior so Local datetimes are accepted.
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Detect production environment (Render sets PORT and RENDER env vars)
var isProduction = builder.Environment.IsProduction()
    || Environment.GetEnvironmentVariable("RENDER") != null
    || Environment.GetEnvironmentVariable("PORT") != null;

if (isProduction)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"Production mode - Port: {port}");
}
else
{
    Console.WriteLine("Development mode - http://localhost:5055");
}

// Add services to the container.
builder.Services.AddControllers();

// Configure Database Context and Logging Levels
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!isProduction)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("DefaultConnection not found in appsettings.json");
        options
            .UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
    else
    {
        // Render provides DATABASE_URL as a URI (postgresql://user:pass@host/db)
        // Npgsql requires key=value format, so we convert it
        var rawUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? builder.Configuration.GetConnectionString("ProductionConnection")
            ?? throw new Exception("Production DB not configured. Set DATABASE_URL in Render.");
        var connectionString = ParseDatabaseUrl(rawUrl);
        options
            .UseNpgsql(connectionString)
            // Suppress type-mapping diff between MySQL snapshot and Npgsql provider
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    }
});

// Configure Lowercase URLS
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Dependency Injection

// // Shared Bounded Context

// Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// //  IoT Device Bounded Context

// Injection Configuration
builder.Services.AddScoped<IIotDeviceRepository, IotDeviceRepository>();
builder.Services.AddScoped<IEdgeDeviceRepository, EdgeDeviceRepository>();
builder.Services.AddScoped<IEdgeRegistryRepository, EdgeRegistryRepository>();

// Agronomic Recommendation Bounded Context Injection Configuration
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IRecommendationCommandService, RecommendationCommandService>();
builder.Services.AddScoped<IRecommendationQueryService, RecommendationQueryService>();

builder.Services.AddScoped<IDeviceStatusCommandService, DeviceStatusCommandService>();
builder.Services.AddScoped<IDeviceStatusQueryService, DeviceStatusQueryService>();
builder.Services.AddScoped<IEdgeSynchronizationService, EdgeSynchronizationService>();

// // Sensor Data Processing Bounded Context

// Injection Configuration
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IAgronomicThresholdRepository, AgronomicThresholdRepository>();

builder.Services.AddScoped<ISensorReadingCommandService, SensorReadingCommandService>();
builder.Services.AddScoped<ISensorReadingQueryService, SensorReadingQueryService>();
builder.Services.AddScoped<IAgronomicThresholdQueryService, AgronomicThresholdQueryService>();
builder.Services.AddScoped<IThresholdEvaluationService, ThresholdEvaluationService>();

// Event Handlers
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining(typeof(IotDeviceRegisteredEventHandler));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Verify Database Objects are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    var logger = services.GetRequiredService<ILogger<Program>>();

    try
    {
        if (isProduction)
        {
            // Production (PostgreSQL): apply migrations from the migration file
            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            logger.LogInformation("Pending migrations: {Pending}", string.Join(", ", pendingMigrations));

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying migrations...");
                context.Database.Migrate();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("No pending migrations found.");
            }
        }
        else
        {
            // Development (MySQL): EnsureCreated derives schema from the model using MySQL types,
            // bypassing the PostgreSQL-typed migration file.
            logger.LogInformation("Development mode: creating MySQL schema from model...");
            context.Database.EnsureCreated();
            logger.LogInformation("Database schema ready.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database schema: {Message}", ex.Message);
        throw;
    }
}

// Configure the HTTP request pipeline.
if (!isProduction)
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartPalm API V1");
    c.RoutePrefix = string.Empty;
});

app.UseCors("AllowAll");

// Render terminates TLS at the load balancer
if (!isProduction)
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
if (isProduction)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    Console.WriteLine($"Swagger: http://0.0.0.0:{port}");
}
else
{
    Console.WriteLine("Swagger: http://localhost:5055/swagger");
}

app.Run();

// Converts postgresql://user:pass@host:port/db  →  Host=...;Username=...;Password=...;Database=...
static string ParseDatabaseUrl(string databaseUrl)
{
    var uri = new Uri(databaseUrl);
    var userInfo = uri.UserInfo.Split(':');
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = Uri.UnescapeDataString(userInfo[1]);
    return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;";
}
