using SQLite;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebAPI_Receta_Core.Models
{
    [Table("Receta")]
    public class Receta
    {
        [PrimaryKey, AutoIncrement]
        public long Id { get; set; }
        public string Nombre { get; set; }
        public string Ingredientes { get; set; }
        public string Instrucciones { get; set; }
        public bool Publica { get; set; }
        public string? Username { get; set; }
    }
}
