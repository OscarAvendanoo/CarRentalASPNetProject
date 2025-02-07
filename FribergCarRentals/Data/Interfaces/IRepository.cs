using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace FribergCarRentals.Data
{
    public interface IRepository<T>
    {
        Task<T> GetByIDAsync(int id);
        Task<T> UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<T> AddAsync(T entity);
        Task SaveChanges();
        Task<IEnumerable<T>> GetAllAsync();
        Task AttachEntity(T entity);
        

    }
}
