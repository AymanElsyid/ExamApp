using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bl.Infrastructure
{
    public interface ITable<T>
    {
        T Get(Guid Id);
        List<T> GetAll();
        bool Delete(Guid Id);
        bool Active(Guid Id);
        bool Update(T obj);
        bool Add(T Obj);
    }
}
