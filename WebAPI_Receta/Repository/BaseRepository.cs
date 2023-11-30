using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using SQLite;

namespace WebAPI_Receta.Repositories
{
    public class BaseRepository<T> : AccesoBaseDatos where T : new()
    {
        protected SQLiteConnection ADB;

        //public BaseRepository(string dbLocation, string password) : base(dbLocation, password)
        //{
        //    new AccesoBaseDatos(dbLocation, password);
        //}

        //public BaseRepository() : base(){}

        public BaseRepository(SQLiteConnection ADB){
            this.ADB = ADB;
        }

        public T FindItems(Expression<Func<T, bool>> predicate)
        {
            var model = ADB.Find<T>(predicate);
            return model;
        }

        public List<T> GetItems() 
        {
            var list = ADB.Table<T>().ToList();
            return list;
        }
        
        public bool Exist(string key)
        {
            bool result;
            try
            {
                var model = ADB.Get<T>(key);
                result = (model != null);
            }
            catch
            {
                result = false;
            }
            
            return result;
        }

        public long DeleteInsertAll<U>(List<U> list)
        {
            long result;
            try
            {
                if(list != null)
                {
                    if (list.Any())
                    {
                        ADB.BeginTransaction();
                        ADB.DeleteAll<U>();
                        result = ADB.InsertAll(list);
                        ADB.Commit();
                        return result;
                    }                    
                }                
            }
            catch (Exception)
            {                
                ADB.Rollback();                
            }
            return 0;
        }

        public long DeleteInsert<U>(U model)
        {
            long result;
            try
            {
                if (model != null)
                {
                    ADB.BeginTransaction();
                    ADB.DeleteAll<U>();
                    result = ADB.Insert(model);
                    ADB.Commit();
                    return result;
                }
            }
            catch (Exception)
            {
                ADB.Rollback();
            }
            return 0;
        }

        public long SaveUpdateItem(T model)
        {
            long result;
            try
            {
                result = ADB.InsertOrReplace(model);
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }        
        public long SaveItem(T model)
        {
            long result;
            result = ADB.Insert(model);
            return result;
        }
        public long SaveAllItem(List<T> list)
        {
            long result;
            result = ADB.InsertAll(list);
            return result;
        }
        public long UpdateItem(T model)
        {
            long result;
            result = ADB.Update(model);
            return result;
        }

        public long UpdateAll(IEnumerable<T> items)
        {
            long result = ADB.UpdateAll(items, true);
            return result;
        }

        public long DeleteAll()
        {
            long result = ADB.DeleteAll<T>();
            return result;
        }
        public long Delete(string key)
        {
            long result;
            try
            {
                result = ADB.Delete(key);
            }
            catch(Exception)
            {
                result = 0;
            }            
            return result;
        }

        public long Delete(object objectToDelete)
        {
            long result;
            try
            {
                result = ADB.Delete(objectToDelete);
            }
            catch (Exception)
            {
                result = 0;
            }
            return result;
        }
    }
}
