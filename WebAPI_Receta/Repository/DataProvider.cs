using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebAPI_Receta.Repositories
{
    public class DataProvider : Repository
    {
        public static DataProvider? Instance { get; set; }
        public static void CreateInstance(string dbPath, string password)
        {
            Instance = new DataProvider(dbPath, password);
        }
        public static DataProvider GetInstance()
        {
            if (Instance == null)
                throw new Exception("No ha creado la instancia");
            return Instance;
        }
        public DataProvider(string dbPath, string password)
        {
            _ = new AccesoBaseDatos(dbPath, password);
        }
    }
}
