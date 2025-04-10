using System;
using System.Text;
using Serilog;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using CurrencyConverterAPI.Helpers;  // <-- ADD THIS LINE

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.WriteTo.Console()
          .ReadFrom.Configuration(context.Configuration);
});

// Essential services
builder.Services.AddControllers();
builder.Services.AddMemoryCache();

// Configure HttpClient for Frankfurter API
builder.Services.AddHttpClient("FrankfurterApi", client =>
{
    client.BaseAddress = new Uri("https://api.frankfurter.dev/v1/");
});

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

builder.Services.AddAuthorization();

// OpenTelemetry tracing configuration
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter());

// Swagger API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware configuration
app.UseSerilogRequestLogging();
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Temporary JWT token generation for testing only
var token = JwtTokenGenerator.GenerateJwtToken(builder.Configuration["Jwt:Key"]);
Console.WriteLine("Your test JWT token (use this in Swagger/Postman):");
Console.WriteLine(token);

app.Run();
