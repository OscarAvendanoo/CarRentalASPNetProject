using System.Linq.Expressions;

namespace FribergCarRentals.Data
{
    public interface IRepository<T>
    {
        T GetbyID(int id);
        T Update(T entity);
        void Delete(T entity);
        T Add(T entity);
        void SaveChanges();
        IEnumerable<T> GetAll();    
     
    }
}
