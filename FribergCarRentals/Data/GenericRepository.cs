using FribergCarRentals.Models;
using Microsoft.EntityFrameworkCore;

namespace FribergCarRentals.Data
{
    public abstract class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;

        protected GenericRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<T> AddAsync(T entity)
        {
            var addedEntity = await _context.AddAsync(entity);
            await _context.SaveChangesAsync();
            return addedEntity.Entity;
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIDAsync(int id)
        {
            return await _context.FindAsync<T>(id);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var updatedEntity = _context.Update(entity).Entity;
            await _context.SaveChangesAsync();
            return updatedEntity;
        }

        public async Task AttachEntity(T entity)
        {
            await Task.Run(() => _context.Attach(entity));
        }
    }
}
