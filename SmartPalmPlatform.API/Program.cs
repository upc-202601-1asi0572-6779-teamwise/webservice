using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SmartPalmPlatform.API.AgronomicRecommendation.Application.CommandServices;
using SmartPalmPlatform.API.AgronomicRecommendation.Application.QueryServices;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Repositories;
using SmartPalmPlatform.API.AgronomicRecommendation.Domain.Services;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.ACL;
using SmartPalmPlatform.API.AgronomicRecommendation.Interfaces.ACL.Services;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.DomainServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.Internal.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Application.OutboundServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Repositories;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.CommandServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Domain.Services.QueryServices;
using SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Firebase.Services;
using SmartPalmPlatform.API.AlertsAndNotifications.Infrastructure.Persistence.EFC.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Application.Internal.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Application.Internal.DomainServices;
using SmartPalmPlatform.API.CropMonitoring.Application.Internal.EventHandlers;
using SmartPalmPlatform.API.CropMonitoring.Application.Internal.QueryServices;
using SmartPalmPlatform.API.CropMonitoring.Application;
using SmartPalmPlatform.API.CropMonitoring.Domain.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.CommandServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.DomainServices;
using SmartPalmPlatform.API.CropMonitoring.Domain.Services.QueryServices;
using SmartPalmPlatform.API.CropMonitoring.Infrastructure.Persistence.EFC.Repositories;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL;
using SmartPalmPlatform.API.CropMonitoring.Interfaces.ACL.Services;
using SmartPalmPlatform.API.FieldTechnicalManagement.Application.Internal.CommandServices;
using SmartPalmPlatform.API.FieldTechnicalManagement.Application.Internal.QueryServices;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Repositories;
using SmartPalmPlatform.API.FieldTechnicalManagement.Domain.Services;
using SmartPalmPlatform.API.FieldTechnicalManagement.Infrastructure.Persistance.EFC.Repositories;
using SmartPalmPlatform.API.IAM.Application.Internal.CommandServices;
using SmartPalmPlatform.API.IAM.Application.Internal.DomainServices;
using SmartPalmPlatform.API.IAM.Application.Internal.OutboundServices;
using SmartPalmPlatform.API.IAM.Application.Internal.QueryServices;
using SmartPalmPlatform.API.IAM.Domain.Model.Aggregates;
using SmartPalmPlatform.API.IAM.Domain.Model.Enums;
using SmartPalmPlatform.API.IAM.Domain.Model.Factory;
using SmartPalmPlatform.API.IAM.Domain.Repositories;
using SmartPalmPlatform.API.IAM.Domain.Services;
using SmartPalmPlatform.API.IAM.Domain.Services.CommandServices;
using SmartPalmPlatform.API.IAM.Domain.Services.DomainServices;
using SmartPalmPlatform.API.IAM.Domain.Services.QueryServices;
using SmartPalmPlatform.API.IAM.Infrastructure.Hashing.BCrypt.Services;
using SmartPalmPlatform.API.IAM.Infrastructure.Persistence.EFC.Repositories;
using SmartPalmPlatform.API.IAM.Infrastructure.Pipeline.Middleware.Extensions;
using SmartPalmPlatform.API.IAM.Infrastructure.Tokens.JWT.Configuration;
using SmartPalmPlatform.API.IAM.Infrastructure.Tokens.JWT.Services;
using SmartPalmPlatform.API.IAM.Interfaces.ACL;
using SmartPalmPlatform.API.IAM.Interfaces.ACL.Services;
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

Console.WriteLine("[INFO] [Startup] SmartPalm Platform API initializing...");
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
Console.WriteLine("[INFO] [Startup] WebApplication builder created.");

// Detect production environment (Render sets PORT and RENDER env vars)
var isProduction =
    builder.Environment.IsProduction()
    || Environment.GetEnvironmentVariable("RENDER") != null
    || Environment.GetEnvironmentVariable("PORT") != null;

if (isProduction)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] [Startup] Production mode — binding to port {port}");
}
else
{
    Console.WriteLine("[INFO] [Startup] Development mode — http://localhost:5055");
}

builder.Services.AddControllers();

Console.WriteLine("[INFO] [Startup] Configuring database context...");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (!isProduction)
    {
        var connectionString =
            builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new Exception("DefaultConnection not found in appsettings.json");
        options
            .UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
        Console.WriteLine("[INFO] [Startup] Using MySQL (development)");
    }
    else
    {
        var rawUrl =
            Environment.GetEnvironmentVariable("DATABASE_URL")
            ?? builder.Configuration.GetConnectionString("ProductionConnection")
            ?? throw new Exception("Production DB not configured. Set DATABASE_URL in Render.");
        var connectionString = ParseDatabaseUrl(rawUrl);
        options
            .UseNpgsql(connectionString)
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
        Console.WriteLine("[INFO] [Startup] Using PostgreSQL (production)");
    }
});

Console.WriteLine("[INFO] [Startup] Registering dependency injection services...");
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IAM Bounded Context
builder.Services.Configure<TokenSettings>(builder.Configuration.GetSection("TokenSettings"));
var tokenSecret = builder.Configuration["TokenSettings:Secret"];
if (string.IsNullOrEmpty(tokenSecret))
{
    Console.WriteLine("[FATAL] [Startup] TokenSettings:Secret is not configured.");
    throw new InvalidOperationException("TokenSettings:Secret is not configured.");
}

// JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(tokenSecret)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = int.Parse(context.Principal!.FindFirst(System.Security.Claims.ClaimTypes.Sid)!.Value);
            var userQueryService = context.HttpContext.RequestServices.GetRequiredService<IUserQueryService>();
            var user = await userQueryService.Handle(new SmartPalmPlatform.API.IAM.Domain.Model.Queries.GetUserByIdQuery(userId));
            if (user != null)
                context.HttpContext.Items["User"] = user;
        }
    };
});
builder.Services.AddAuthorization();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
builder.Services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();
builder.Services.AddScoped<IHashingService, HashingService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserCommandService, UserCommandService>();
builder.Services.AddScoped<IUserQueryService, UserQueryService>();
builder.Services.AddScoped<IUserDomainService, UserDomainService>();
builder.Services.AddScoped<ISubscriptionCommandService, SubscriptionCommandService>();
builder.Services.AddScoped<IPaymentCommandService, PaymentCommandService>();
builder.Services.AddScoped<ISubscriptionQueryService, SubscriptionQueryService>();
builder.Services.AddScoped<IPaymentQueryService, PaymentQueryService>();
builder.Services.AddScoped<
    ISubscriptionLifecycleDomainService,
    SubscriptionLifecycleDomainService
>();
builder.Services.AddScoped<IPaymentStrategy, LocalPaymentStrategy>();
builder.Services.AddScoped<IIamContextFacade, IamContextFacade>();

// IoT Device Bounded Context
builder.Services.AddScoped<IIotDeviceRepository, IotDeviceRepository>();
builder.Services.AddScoped<IEdgeDeviceRepository, EdgeDeviceRepository>();
builder.Services.AddScoped<IEdgeRegistryRepository, EdgeRegistryRepository>();

// Agronomic Recommendation Bounded Context Injection Configuration
builder.Services.AddScoped<IRecommendationRepository, RecommendationRepository>();
builder.Services.AddScoped<IRecommendationCommandService, RecommendationCommandService>();
builder.Services.AddScoped<IRecommendationQueryService, RecommendationQueryService>();
builder.Services.AddScoped<IRecommendationFacade, RecommendationFacade>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportCommandService, ReportCommandService>();
builder.Services.AddScoped<IReportQueryService, ReportQueryService>();

// Field Technical Management Bounded Context Injection Configuration
builder.Services.AddScoped<IAgronomicInterventionRepository, JpaAgronomicInterventionRepository>();
builder.Services.AddScoped<
    IAgronomicInterventionCommandService,
    AgronomicInterventionCommandService
>();
builder.Services.AddScoped<IAgronomicInterventionQueryService, AgronomicInterventionQueryService>();

builder.Services.AddScoped<IDeviceStatusCommandService, DeviceStatusCommandService>();
builder.Services.AddScoped<IDeviceStatusQueryService, DeviceStatusQueryService>();
builder.Services.AddScoped<IEdgeSynchronizationService, EdgeSynchronizationService>();

// Sensor Data Processing Bounded Context
builder.Services.AddScoped<ISensorReadingRepository, SensorReadingRepository>();
builder.Services.AddScoped<IAgronomicThresholdRepository, AgronomicThresholdRepository>();

builder.Services.AddScoped<ISensorReadingCommandService, SensorReadingCommandService>();
builder.Services.AddScoped<ISensorReadingQueryService, SensorReadingQueryService>();
builder.Services.AddScoped<IAgronomicThresholdQueryService, AgronomicThresholdQueryService>();
builder.Services.AddScoped<ISectorHealthQueryService, SectorHealthQueryService>();
builder.Services.AddScoped<IThresholdEvaluationService, ThresholdEvaluationService>();
builder.Services.AddScoped<ISensorTypeDomainService, SensorTypeDomainService>();

// Alerts & Notifications Bounded Context
builder.Services.AddScoped<IAlertRepository, AlertRepository>();
builder.Services.AddScoped<IUserAlertSettingRepository, UserAlertSettingRepository>();
builder.Services.AddScoped<IAlertCommandService, AlertCommandService>();
builder.Services.AddScoped<IAlertQueryService, AlertQueryService>();
builder.Services.AddScoped<IUserAlertSettingCommandService, UserAlertSettingCommandService>();
builder.Services.AddScoped<IUserAlertSettingQueryService, UserAlertSettingQueryService>();
builder.Services.AddScoped<AlertClassificationService>();
builder.Services.AddScoped<IFirebaseNotificationService, FirebaseNotificationService>();

// Crop Monitoring Bounded Context
builder.Services.AddScoped<IPlantationRepository, PlantationRepository>();
builder.Services.AddScoped<ISectorRepository, SectorRepository>();
builder.Services.AddScoped<IAgronomistPlantationAffiliationRepository, AgronomistPlantationAffiliationRepository>();
builder.Services.AddScoped<IPlantationCommandService, PlantationCommandService>();
builder.Services.AddScoped<IPlantationQueryService, PlantationQueryService>();
builder.Services.AddScoped<IInstallationPlanService, InstallationPlanService>();
builder.Services.AddScoped<IAgronomistPlantationAffiliationCommandService, AgronomistPlantationAffiliationCommandService>();
builder.Services.AddScoped<IAgronomistPlantationAffiliationQueryService, AgronomistPlantationAffiliationQueryService>();
builder.Services.AddScoped<ICropMonitoringFacade, CropMonitoringFacade>();

Console.WriteLine("[INFO] [Startup] DI registrations complete. Registering event handlers...");

// Event Handlers
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblyContaining(
        typeof(CropMonitoringIotDeviceRegisteredEventHandler)
    );
    config.RegisterServicesFromAssemblyContaining(typeof(ThresholdExceededEventHandler));
    config.RegisterServicesFromAssemblyContaining(typeof(IotDeviceRegisteredEventHandler));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => options.EnableAnnotations());

// Add authentication with Bearer token support for JWT
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter your token in the text input below.\r\n\r\nExample: \"12345abcdef\"",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
        }
    );

    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() },
    });
});

var app = builder.Build();
Console.WriteLine("[INFO] [Startup] Application built. Initializing database...");

// Verify Database Objects are created
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();

    try
    {
        Console.WriteLine("[INFO] [Database] Ensuring MySQL schema is created...");
        context.Database.EnsureCreated();
        Console.WriteLine("[INFO] [Database] Schema ready.");

        Console.WriteLine("[INFO] [Database] Running seed...");
        SeedAdmin(context);
        Console.WriteLine("[INFO] [Database] Seed complete.");
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine(
            $"[ERROR] [Database] Schema error: {ex.GetType().Name}: {ex.Message}"
        );
        throw;
    }
}

static void SeedAdmin(AppDbContext context)
{
    Console.WriteLine("[INFO] [Seed] Checking admin user...");
    if (!context.Set<User>().Any(u => u.Role == UserRole.Administrator))
    {
        var admin = new User(
            "admin",
            BCrypt.Net.BCrypt.HashPassword("admin"),
            "admin@smartpalm.com",
            "System Administrator",
            UserRole.Administrator
        );
        context.Set<User>().Add(admin);
        context.SaveChanges();
        Console.WriteLine("[INFO] [Seed] Admin user created (admin/admin).");
    }
    else
    {
        Console.WriteLine("[INFO] [Seed] Admin user already exists, skipping.");
    }
}

Console.WriteLine("[INFO] [Startup] Configuring HTTP pipeline...");

// Configure the HTTP request pipeline.
if (!isProduction)
{
    app.MapOpenApi();
    Console.WriteLine("[INFO] [Startup] OpenAPI mapped (development).");
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SmartPalm API V1");
    c.RoutePrefix = string.Empty;
});
Console.WriteLine("[INFO] [Startup] Swagger UI configured.");

app.UseCors("AllowAll");
Console.WriteLine("[INFO] [Startup] CORS policy 'AllowAll' applied.");

// Render terminates TLS at the load balancer
if (!isProduction)
{
    app.UseHttpsRedirection();
    Console.WriteLine("[INFO] [Startup] HTTPS redirect enabled.");
}

// Request authorization middleware must come before UseAuthorization()
app.UseAuthentication();
app.UseRequestAuthorization();
Console.WriteLine("[INFO] [Startup] Request authorization middleware registered.");

app.UseAuthorization();

// Apply authorization to all controllers (except sign-in/sign-up which are handled by IAM)
app.MapControllers();
Console.WriteLine("[INFO] [Startup] Controllers mapped.");

Console.WriteLine($"[INFO] [Startup] Environment: {app.Environment.EnvironmentName}");
if (isProduction)
{
    var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
    Console.WriteLine($"[INFO] [Startup] Listening on http://0.0.0.0:{port}");
    Console.WriteLine($"[INFO] [Startup] Swagger at http://0.0.0.0:{port}");
}
else
{
    Console.WriteLine("[INFO] [Startup] Listening on http://localhost:5055");
    Console.WriteLine("[INFO] [Startup] Swagger at http://localhost:5055/swagger");
}

Console.WriteLine("[INFO] [Startup] Application started.");
app.Run();
Console.WriteLine("[INFO] [Shutdown] Application stopped.");

// Converts postgresql://user:pass@host:port/db → Host=...;Username=...;Password=...;Database=...
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
