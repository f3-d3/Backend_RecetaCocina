using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPI_Receta.Auth
{
    public class JwtAuthenticationService : IJwtAuthenticationService
    {

        private readonly string _key;

        public JwtAuthenticationService (string key)
        {
            _key = key;
        }

        public string Authenticate(string username, string password)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            // Convertir la clave en bytes para su uso en la firma del token.
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            // Configurar la descripción del token, que incluye la identidad del usuario, la caducidad y la firma.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), 
                SecurityAlgorithms.HmacSha256Signature)
            };

            // Crear el token utilizando el descriptor configurado.
            var token = tokenHandler.CreateToken(tokenDescriptor);

           return tokenHandler.WriteToken(token);

        }

    }
}
