using Microsoft.EntityFrameworkCore;
using SmartPalmPlatform.API.IotDeviceManagement.Application.CommandServices;
using SmartPalmPlatform.API.IotDeviceManagement.Application.QueryServices;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Repositories;
using SmartPalmPlatform.API.IotDeviceManagement.Domain.Services;
using SmartPalmPlatform.API.Shared.Domain.Repositories;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Configuration;
using SmartPalmPlatform.API.Shared.Infrastructure.Persistence.EFC.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configure Database Context and Logging Levels
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        if (connectionString is null)
            throw new Exception("Connection string not found");
        options
            .UseMySQL(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("ProductionConnection");
        if (connectionString is null)
            throw new Exception("Connection string not found");
        options
            .UseNpgsql(connectionString)
            .LogTo(Console.WriteLine, LogLevel.Error)
            .EnableDetailedErrors();
    }
});

// Configure Lowercase URLS
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Configure Dependency Injection

// Shared Bounded Context Injection Configuration
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// IoT Device Bounded Context Injection Configuration
builder.Services.AddScoped<IIotDeviceRepository, IotDeviceRepository>();

builder.Services.AddScoped<IDeviceStatusCommandService, DeviceStatusCommandService>();
builder.Services.AddScoped<IDeviceStatusQueryService, DeviceStatusQueryService>();

builder.Services.AddScoped<IEdgeSynchronizationService, EdgeSyncrhonizationService>();

// CQRS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
        }
    );
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
    context.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// add swagger
// if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

// Add Authorization Middleware to Pipeline
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
