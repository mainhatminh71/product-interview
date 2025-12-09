using Microsoft.EntityFrameworkCore;
using Product.BLL.Services;
using Product.DAL.Data;
using Product.DAL.Repo;
using SendGrid;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddTransient<IAccountRepo, AccountRepo>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IOtpService, MemoryOtpService>();
builder.Services.AddMemoryCache();

// Đọc SendGrid API Key từ environment variable hoặc configuration
var sendGridApiKey = Environment.GetEnvironmentVariable("SENDGRID_EMAIL_API_KEY") 
    ?? Environment.GetEnvironmentVariable("SendGrid__ApiKey")
    ?? builder.Configuration["SendGrid:ApiKey"];

if (string.IsNullOrWhiteSpace(sendGridApiKey))
{
    throw new InvalidOperationException("SendGrid API Key is required. Please set SENDGRID_EMAIL_API_KEY or SendGrid__ApiKey environment variable, or configure SendGrid:ApiKey in appsettings.json");
}

builder.Services.AddSingleton<SendGridClient>(sp =>
    new SendGridClient(sendGridApiKey));


// Đọc connection string từ environment variable DATABASE_URL (Render) hoặc từ appsettings.json
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    // Nếu không có DATABASE_URL, đọc từ appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

// Parse DATABASE_URL từ format postgresql://user:password@host:port/database
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgresql://"))
{
    // Parse PostgreSQL URL format: postgresql://user:password@host:port/database
    var uri = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    var username = Uri.UnescapeDataString(userInfo[0]);
    var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
    var host = uri.Host;
    var port = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');
    
    // Tạo connection string theo format Npgsql
    connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true;Timeout=60;Command Timeout=60";
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is required. Please set DATABASE_URL environment variable or configure DefaultConnection in appsettings.json");
}

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    try
    {
        // Áp dụng các EF Core migrations (tự tạo/tự cập nhật schema, bao gồm cột GoogleId)
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Log lỗi nếu có vấn đề với migration
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
