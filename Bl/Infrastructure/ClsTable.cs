using Bl.Enumration;
using Domain;
using Domain.Tables;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Infrastructure
{
    public class ClsTable<T> : ITable<T> where T : BaseTable
    {
        ExamAppDbContext Db = new ExamAppDbContext();
        public ClsTable()
        {
            
        }
        public bool Add(T Obj)
        {
            try
            {
                Obj.Id = Guid.NewGuid();
                Db.Set<T>().Add(Obj);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Delete(Guid Id)
        {
            try
            {
                var Obj = Db.Set<T>().Find(Id);
                Obj.CurrentState = (int)CurrentStateEnum.Deleted;
                Db.Set<T>().Update(Obj);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public bool Active(Guid Id)
        {
            try
            {
                var Obj = Db.Set<T>().Find(Id);
                Obj.CurrentState = (int)CurrentStateEnum.Active;
                Db.Set<T>().Update(Obj);
                Db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public T Get(Guid Id)
        {
            try
            {
                var Obj = Db.Set<T>().Find(Id);
                return Obj;
            }
            catch
            {
                return new BaseTable() as T;
            }
        }

        public List<T> GetAll()
        {
            try
            {
                var Objts = Db.Set<T>().ToList();
                return Objts;
            }
            catch
            {
                return new List<BaseTable>() as List<T>;
            }
        }

        public bool Update(T obj)
        {
            try
            {
                var OldObj = Db.Set<T>().Find(obj.Id);
                if (OldObj != null)
                {
                    Db.Set<T>().Update(obj);
                    Db.SaveChanges();
                    return true;
                }
                else
                    return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
