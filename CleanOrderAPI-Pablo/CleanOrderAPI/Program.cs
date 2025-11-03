using CleanOrderAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CleanOrderAPI.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Explicitly set camelCase (this is actually the default in .NET 8)
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Optional: Make property names case-insensitive for incoming JSON
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();

// ✅ Swagger con configuración de seguridad JWT
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CleanOrderAPI",
        Version = "v1",
        Description = "API para la gestión de órdenes, empleados y vehículos (CleanOrder)."
    });

    // 🔐 Configuración de esquema de seguridad (JWT)
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce el token JWT con el prefijo **Bearer**. Ejemplo: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
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
            new string[] {}
        }
    };

    c.AddSecurityRequirement(securityRequirement);
});

// Register ApplicationDbContext with MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(10, 4, 32)),
        mySqlOptions => mySqlOptions.EnableStringComparisonTranslations()
    ));

builder.Services.AddSingleton<JWTService>();
builder.Services.AddScoped<PasswordService>();
builder.Services.AddScoped<IEmailValidationService, EmailValidationService>();

// CORS for Angular/Ionic
builder.Services.AddCors(options =>
{
    options.AddPolicy("AngularDevClient", policy =>
    {
        policy.WithOrigins(
                "http://localhost:4200",
                "https://localhost:4200",
                "http://localhost:8100",
                "https://localhost:8100",
                "http://localhost:8101",
                "https://localhost:8101"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Configure JWT Authentication (read from cookie)
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
            // Extrae token desde la cookie
            if (context.Request.Cookies.TryGetValue(JwtCookieName, out var token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            // Refresca el token si quedan menos de 10 minutos
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// CORS debe ir antes de la autenticación
app.UseCors("AngularDevClient");

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
