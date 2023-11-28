using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using WebAPI_Receta.Models;
using WebAPI_Receta.Repositories;
using Microsoft.AspNetCore.Hosting;
using WebAPI_Receta.Auth;
using static System.Collections.Specialized.BitVector32;

namespace TokenApi.Controllers
{
    /// <summary>
    /// Controlador para gestionar la autenticación y registro de usuarios mediante tokens JWT.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IJwtAuthenticationService _authService;

        /// <summary>
        /// Constructor del controlador TokenController.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="webHostEnvironment">Entorno de alojamiento web.</param>
        /// <param name="authService">Servicio de autenticación JWT.</param>
        public TokenController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IJwtAuthenticationService authService)
        {
            _authService = authService;

            // Configuración de la conexión a la base de datos SQLite.
            var SQLite_Cipher = configuration.GetSection("SQLite_Cipher").Value;
            var ruta = Path.Combine(webHostEnvironment.ContentRootPath, "RecetaV1.db3");
            DataProvider.CreateInstance(ruta, SQLite_Cipher);
            Repository.SetConexion(ruta, SQLite_Cipher);
        }

        /// <summary>
        /// Método para autenticar a un usuario y generar un token JWT.
        /// </summary>
        /// <param name="user">Información de autenticación del usuario.</param>
        /// <returns>Resultado de la autenticación y token JWT en caso de éxito.</returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] AuthInfo user)
        {
            string token = null;
            try
            {
                var authInfos = Repository._AuthInfo.GetItems();
                if (authInfos != null)
                {
                    user.Username = user.Username.ToUpper();
                    var authInfo = authInfos.FirstOrDefault(x => x.Username.Equals(user.Username));

                    if (authInfo != null)
                    {
                        token = _authService.Authenticate(user.Username, user.Password);
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return Conflict(ex);
            }

            if (token == null)
            {
                return Unauthorized();
            }
            return Ok(token);
        }

        /// <summary>
        /// Método para registrar a un nuevo usuario.
        /// </summary>
        /// <param name="user">Información de registro del usuario.</param>
        /// <returns>Resultado del registro y los detalles del usuario registrado.</returns>
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] AuthInfo user)
        {
            try
            {
                user.Username = user.Username.ToUpper();
                var authInfos = Repository._AuthInfo.GetItems();
                if (authInfos != null)
                {
                    var authInfo = authInfos.FirstOrDefault(x => x.Username.Equals(user.Username));

                    if (authInfo != null)
                    {
                        return Conflict("Usuario ya fue creado");
                    }
                    else
                    {
                        Repository._AuthInfo.SaveItem(user);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                Repository.ADBRollback();
                return Conflict(ex);
            }
            return Created(string.Empty, user);
        }
    }
}
