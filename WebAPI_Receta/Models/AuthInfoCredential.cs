using SQLite;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Receta.Models
{
    public class AuthInfoCredential
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [MinLength(8, ErrorMessage = "El nombre de usuario debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "El nombre de usuario debe contener al menos una mayúscula y un número.")]
        [PrimaryKey]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        //[RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "La contraseña debe contener al menos una mayúscula y un número.")]
        public string Password { get; set; }
    }
}
