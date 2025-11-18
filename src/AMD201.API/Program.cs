using AMD201.Core.Interfaces;
using AMD201.Infrastructure.Data;
using AMD201.Infrastructure.Repositories;
using AMD201.Infrastructure.Services;
using AMD201.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "AMD201 URL Shortener API",
        Version = "v1",
        Description = "A high-performance URL shortening service with Supabase authentication"
    });
});

// Database Configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    // Use PostgreSQL for production (Render), SQLite for development
    connectionString = builder.Environment.IsDevelopment()
        ? "Data Source=urlshortener.db"
        : Environment.GetEnvironmentVariable("DATABASE_URL");
}

// Convert Heroku/Render PostgreSQL URL format if needed
if (!string.IsNullOrEmpty(connectionString) && connectionString.StartsWith("postgres://"))
{
    var uri = new Uri(connectionString);
    connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.Trim('/')};Username={uri.UserInfo.Split(':')[0]};Password={uri.UserInfo.Split(':')[1]};SSL Mode=Require;Trust Server Certificate=true";
}

if (connectionString?.Contains("Data Source") == true)
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite(connectionString));
}
else
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));
}

// JWT Authentication Configuration for Supabase
var supabaseJwtSecret = builder.Configuration["Supabase:JwtSecret"] 
    ?? Environment.GetEnvironmentVariable("SUPABASE_JWT_SECRET")
    ?? "your-supabase-jwt-secret-here"; // Default for development

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(supabaseJwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register Services
builder.Services.AddScoped<IUrlRepository, UrlRepository>();
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddSingleton<IQrCodeService, QrCodeService>();

var app = builder.Build();

// Auto-migrate database on startup (only in production)
try
{
    if (!app.Environment.IsDevelopment())
    {
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Running database migrations...");
            db.Database.Migrate();
            logger.LogInformation("Database migrations completed.");
        }
    }
    else
    {
        // In development, ensure database is created
        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("Ensuring database is created...");
            db.Database.EnsureCreated();
            logger.LogInformation("Database ready.");
        }
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while setting up the database.");
    // Don't crash the app, continue anyway
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AMD201 URL Shortener API v1");
    });
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAll");

// Serve static files (Frontend)
var staticFileOptions = new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = new Microsoft.AspNetCore.Http.PathString("")
};
app.UseStaticFiles(staticFileOptions);
app.UseDefaultFiles();

// Custom middleware for Supabase authentication
app.UseMiddleware<SupabaseAuthMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    version = "1.0.0"
}));

// Fallback to index.html for SPA routing
app.MapFallbackToFile("index.html");

app.Run();

