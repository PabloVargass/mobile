using CleanOrderAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanOrderAPI.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// === Add services to the container ===
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // ✅ Establecer formato camelCase (por defecto en .NET 8)
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // ✅ Permitir mayúsculas/minúsculas indiferentes en JSON de entrada
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// === Register ApplicationDbContext con MySQL ===
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 4, 32)),
        mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
    ));

// === Servicios personalizados ===
builder.Services.AddSingleton<JWTService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IEmailValidationService, EmailValidationService>();

// === Configuración de CORS ===
// Debe coincidir con los orígenes de tus apps (Angular / Ionic)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",  // Angular local
            "https://localhost:4200",
            "http://localhost:8100",  // Ionic local
            "https://localhost:8100"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials(); // ✅ Necesario porque usas withCredentials:true
    });
});

// === Configuración JWT Authentication (desde cookie) ===
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);
const string JwtCookieName = "AuthToken";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // ✅ Extraer token desde cookie
            if (context.Request.Cookies.TryGetValue(JwtCookieName, out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // ✅ Renovación automática si el token expira en menos de 10 minutos
            if (context.SecurityToken is JwtSecurityToken jwt)
            {
                var remaining = jwt.ValidTo.ToUniversalTime() - DateTime.UtcNow;
                if (remaining < TimeSpan.FromMinutes(10))
                {
                    var jwtService = context.HttpContext.RequestServices.GetRequiredService<JWTService>();

                    var correo = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value ?? "";
                    var userIdStr = context.Principal?.FindFirst("UserId")?.Value ?? "0";
                    var roleStr = context.Principal?.FindFirst(ClaimTypes.Role)?.Value ?? "0";
                    int.TryParse(userIdStr, out var userId);
                    int.TryParse(roleStr, out var roleId);

                    var newToken = jwtService.GenerateJwtToken(new CleanOrderAPI.Data.Entities.Usuario
                    {
                        IdUsuario = userId,
                        Correo = correo,
                        FkIdRol = roleId,
                        Password = "",
                        Activo = 1
                    });

                    context.Response.Cookies.Append(JwtCookieName, newToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTimeOffset.UtcNow.AddHours(1),
                        Path = "/",
                        IsEssential = true
                    });
                }
            }
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        RoleClaimType = ClaimTypes.Role
    };
});

builder.Services.AddAuthorization();

// === Build app ===
var app = builder.Build();

// ✅ CORS debe ir antes de autenticación
app.UseCors("AllowFrontend");

// === Pipeline de desarrollo ===
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
