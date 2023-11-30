using SQLite;
using WebAPI_Receta.Models;
using WebAPI_Receta_Core.Models;

namespace WebAPI_Receta.Repositories
{
    public class AccesoBaseDatos
    {
        public AccesoBaseDatos(string dbPath, string password)
        {
            CreateTable(dbPath, password);
        }

        public AccesoBaseDatos()
        {
        }

        private void CreateTable(string dbPath, string password)
        {
            using var ABD = new SQLiteConnection(dbPath);
            ABD.Execute($"PRAGMA key = '{password}';");
            ABD.CreateTable<AuthInfo>();
            ABD.CreateTable<Receta>();
        }
    }
}
