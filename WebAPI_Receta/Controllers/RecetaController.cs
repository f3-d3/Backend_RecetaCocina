using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokenApi.Controllers;
using WebAPI_Receta.Auth;
using WebAPI_Receta.Repositories;
using WebAPI_Receta_Core.Models;

namespace WebAPI_Receta.Controllers
{
    /// <summary>
    /// Controlador para manejar las operaciones CRUD relacionadas con las recetas.
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RecetaController : ControllerBase
    {
        /// <summary>
        /// Constructor del controlador RecetaController.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación.</param>
        /// <param name="webHostEnvironment">Entorno web de la aplicación.</param>
        public RecetaController(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            var SQLite_Cipher = configuration.GetSection("SQLite_Cipher").Value;

            var ruta = Path.Combine(webHostEnvironment.ContentRootPath, "RecetaV1.db3");
            DataProvider.CreateInstance(ruta, SQLite_Cipher);
            Repository.SetConexion(ruta, SQLite_Cipher);
        }

        /// <summary>
        /// Obtiene todas las recetas disponibles, filtradas según la visibilidad y el usuario actual.
        /// </summary>
        /// <returns>Lista de recetas.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var listReceta = new List<Receta>();
            try
            {
                var user = GetUsername();
                listReceta = Repository._Receta.GetItems().Where(x => x.Publica || (!x.Publica && x.Username.Equals(user))).ToList();

                if (!listReceta.Any())
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return Conflict(ex);
            }

            return Ok(listReceta);
        }

        /// <summary>
        /// Obtiene una receta por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único de la receta.</param>
        /// <returns>La receta con el identificador proporcionado.</returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetById(long Id)
        {
            var citaMedica = new Receta();
            try
            {
                citaMedica = Repository._Receta.GetItems().FirstOrDefault(x => x.Id == Id);
                
                if (citaMedica == null)
                {
                    return NoContent();
                }
            }
            catch (Exception ex)
            {
                return Conflict(ex);
            }

            return Ok(citaMedica);
        }

        /// <summary>
        /// Crea una nueva receta.
        /// </summary>
        /// <param name="receta">Datos de la receta a crear.</param>
        /// <returns>La receta creada.</returns>
        [HttpPost]
        public async Task<IActionResult> Post(Receta receta)
        {
            try
            {
                receta.Username = GetUsername();
                Repository._Receta.SaveItem(receta);

                long id = Repository._Receta.GetItems().Max(x => x.Id);
                receta.Id = id;
            }
            catch (Exception ex)
            {
                return Conflict(ex);
            }

            return Created(string.Empty, receta);
        }

        /// <summary>
        /// Actualiza una receta existente.
        /// </summary>
        /// <param name="receta">Datos actualizados de la receta.</param>
        /// <returns>La receta actualizada.</returns>
        [HttpPut]
        public async Task<IActionResult> Put(Receta receta)
        {
            try
            {
                var recetaDB = Repository._Receta.GetItems();
                if (recetaDB != null && recetaDB.Any())
                {
                    receta.Username = GetUsername();
                    Repository._Receta.UpdateItem(receta);
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

            return Ok(receta);
        }

        /// <summary>
        /// Elimina una receta por su identificador único.
        /// </summary>
        /// <param name="Id">Identificador único de la receta a eliminar.</param>
        /// <returns>Identificador de la receta eliminada.</returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(long Id)
        {
            try
            {
                var recetaDB = Repository._Receta.GetItems();
                if (recetaDB != null)
                {
                    var receta = recetaDB.FirstOrDefault(x => x.Id == Id);
                    
                    if (receta != null)
                    {
                        long output = Repository._Receta.Delete(receta);
                        if (output == 0)
                        {
                            return NotFound();
                        }
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

            return Ok(Id);
        }

        /// <summary>
        /// Obtiene el nombre de usuario a partir del encabezado "user" de la solicitud.
        /// </summary>
        /// <returns>Nombre de usuario.</returns>
        private string GetUsername()
        {
            Microsoft.Extensions.Primitives.StringValues user;
            _ = Request.Headers.TryGetValue("user", out user);
            return user.ToString().ToUpper();
        }

    }
}
