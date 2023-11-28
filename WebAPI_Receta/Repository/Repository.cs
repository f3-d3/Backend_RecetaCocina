using SQLite;
using System.Diagnostics;
using WebAPI_Receta.Models;
using WebAPI_Receta_Core.Models;

namespace WebAPI_Receta.Repositories
{
    public class Repository 
    {
        #region Base Datos
        private static SQLiteConnection _ADB = SetConexion(string.Empty, string.Empty);

        public static SQLiteConnection SetConexion(string dataBaseFilePathTransaccional, string password)
        {
            if (!string.IsNullOrEmpty(dataBaseFilePathTransaccional) && !string.IsNullOrEmpty(password) && _ADB == null)
            {
                _ADB = new SQLiteConnection(dataBaseFilePathTransaccional);
                _ADB.Execute($"PRAGMA key = '{password}';");
            }
            return _ADB;
        }

        public static void ADBBeginTransaction()
        {
            _ADB.BeginTransaction();
        }

        public static void ADBCommit()
        {
            _ADB.Commit();
        }

        public static void ADBRollback()
        {
            _ADB.Rollback();
        }
        #endregion 
        private static BaseRepository<AuthInfo> _authInfo = _ADB == null ? null : new BaseRepository<AuthInfo>(_ADB);
        private static BaseRepository<Receta> _receta = _ADB == null ? null : new BaseRepository<Receta>(_ADB);

        #region Repositorios

        public static BaseRepository<AuthInfo> _AuthInfo
        {
            get
            {
                if (_authInfo == null)
                {
                    _authInfo = new BaseRepository<AuthInfo>(_ADB);
                }
                return _authInfo;
            }
        }

        public static BaseRepository<Receta> _Receta
        {
            get
            {
                if (_receta == null)
                {
                    _receta = new BaseRepository<Receta>(_ADB);
                }
                return _receta;
            }
        }

        #endregion

    }
}
