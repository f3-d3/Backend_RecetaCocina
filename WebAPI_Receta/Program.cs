using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI_Receta.Auth;

var builder = WebApplication.CreateBuilder(args);

// Obtener la clave secreta para la autenticaci�n JWT desde la configuraci�n.
var key = builder.Configuration.GetSection("JWTKey").Value;

// A�adir servicios al contenedor de dependencias.
builder.Services.AddControllers().
    AddJsonOptions(
    options =>
    {
        // Configurar la serializaci�n JSON para que sea m�s legible.
        options.JsonSerializerOptions.WriteIndented = true;
    }
    );

// Configurar la autenticaci�n JWT.
builder.Services.AddAuthentication(
    x=>
    {
        // Configurar el esquema de autenticaci�n predeterminado.
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    }).
    AddJwtBearer(
    x=>
    {
        // Configuraciones adicionales para la autenticaci�n JWT.
        x.RequireHttpsMetadata = true;
        x.SaveToken = true;

        // Par�metros de validaci�n del token JWT.
        x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateIssuer = false
        };
    });

// Configurar la autorizaci�n.
builder.Services.AddAuthorization();
// Configurar Swagger/OpenAPI para la documentaci�n de la API.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar el servicio de autenticaci�n JWT como un servicio singleton.
builder.Services.AddSingleton<IJwtAuthenticationService>(new JwtAuthenticationService(key));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Habilitar Swagger en entornos de desarrollo.
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Habilitar la autenticaci�n y autorizaci�n.
app.UseAuthentication();
app.UseAuthorization();

// Mapear los controladores de la API.
app.MapControllers();

app.Run();
