using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI_Receta.Auth;

var builder = WebApplication.CreateBuilder(args);

// Obtener la clave secreta para la autenticación JWT desde la configuración.
var key = builder.Configuration.GetSection("JWTKey").Value;
var tokenExpireHours = 1;
int.TryParse(builder.Configuration.GetSection("TokenExpireHours").Value, out tokenExpireHours);

// Añadir servicios al contenedor de dependencias.
builder.Services.AddControllers().
    AddJsonOptions(
    options =>
    {
        // Configurar la serialización JSON para que sea más legible.
        options.JsonSerializerOptions.WriteIndented = true;
    }
    );

// Configurar la autenticación JWT.
builder.Services.AddAuthentication(
    x=>
    {
        // Configurar el esquema de autenticación predeterminado.
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).
    AddJwtBearer(
    x=>
    {
        // Configuraciones adicionales para la autenticación JWT.
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;

        // Parámetros de validación del token JWT.
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false
        };
    });

// Configurar la autorización.
builder.Services.AddAuthorization();
// Configurar Swagger/OpenAPI para la documentación de la API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar el servicio de autenticación JWT como un servicio singleton.
builder.Services.AddSingleton<IJwtAuthenticationService>(new JwtAuthenticationService(key, tokenExpireHours));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger en entornos de desarrollo.
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Habilitar la autenticación y autorización.
app.UseAuthentication();
app.UseAuthorization();

// Mapear los controladores de la API.
app.MapControllers();

app.Run();
