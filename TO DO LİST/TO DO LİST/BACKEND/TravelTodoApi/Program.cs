using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TravelTodoApi.Data;
using TravelTodoApi.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;  // Döngüleri engellemek için
    });

// Add SQLite as the database provider
builder.Services.AddDbContext<TravelTodoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// JWT Authentication
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Secret"]);  // JWT Secret key from appsettings

// Ensure the key length is at least 256 bits (32 bytes)
if (key.Length < 32)
{
    throw new ArgumentException("JWT Secret key must be at least 256 bits (32 bytes) long.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,     // Consider setting ValidateIssuer to true and using a proper issuer
            ValidateAudience = false,   // Consider setting ValidateAudience to true and using a proper audience
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero   // No grace period for token expiration
        };
    });

// Add TokenService to DI container for JWT token generation
builder.Services.AddSingleton<TokenService>();

// Build the app
var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");  // Enable CORS
app.UseAuthentication();  // Enable JWT authentication
app.UseAuthorization();   // Enable authorization

app.MapControllers(); // Map controllers to routes
app.Run();
