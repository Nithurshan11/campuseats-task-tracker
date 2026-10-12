using System.Text;
using CampusEats.Api.Data;
using CampusEats.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configure dynamic port (supports Render PORT env variable)
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Database Context (PostgreSQL with Neon & Render support)
var connectionString = GetConnectionString(builder.Configuration);
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(connectionString));

// Scoped Service
builder.Services.AddScoped<IMenuService, MenuService>();

// CORS configuration (Lab 11 Task 03)
builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
    p.SetIsOriginAllowed(origin => true) // allow Netlify frontend, local dev, preview
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowCredentials()));

// JWT Authentication (Lab 04 Task 04)
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["Key"] ?? "dev-only-secret-change-me-min-32-characters!!");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt => opt.TokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidIssuer = jwt["Issuer"] ?? "CampusEats",
        ValidateAudience = true,
        ValidAudience = jwt["Audience"] ?? "CampusEatsUsers",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateLifetime = true
    });

builder.Services.AddAuthorization();

// Swagger with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Paste ONLY the token - no 'Bearer ' prefix."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Enable Swagger in Development and Cloud environments
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGet("/", () => Results.Ok(new { message = "CampusEats API is running", status = "Healthy" }));

// Auto-apply pending migrations on Startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database migration notice: {ex.Message}");
    }
}

app.Run();

// Helper to support both standard Npgsql key-value format and postgresql:// URI format (e.g. from Neon or Render)
static string GetConnectionString(IConfiguration configuration)
{
    var raw = Environment.GetEnvironmentVariable("DATABASE_URL")
              ?? configuration.GetConnectionString("Default")
              ?? "";

    if (string.IsNullOrWhiteSpace(raw)) return raw;

    if (raw.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
        raw.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var uri = new Uri(raw);
            var userInfo = uri.UserInfo.Split(':');
            var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var host = uri.Host;
            var port = uri.Port > 0 ? uri.Port : 5432;
            var database = uri.AbsolutePath.TrimStart('/');

            var npgsqlBuilder = new Npgsql.NpgsqlConnectionStringBuilder
            {
                Host = host,
                Port = port,
                Database = database,
                Username = username,
                Password = password,
                SslMode = Npgsql.SslMode.Require
            };
            return npgsqlBuilder.ConnectionString;
        }
        catch
        {
            return raw;
        }
    }

    return raw;
}
