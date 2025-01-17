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
        public T Add(T entity)
        {
            var addedEntity = _context.Add(entity).Entity;
            _context.SaveChanges();
            return entity;
        }

        public void Delete(T entity)
        {
            var deletedEntity = _context.Remove(entity).Entity;
            _context.SaveChanges();
       
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetbyID(int id)
        {
            return _context.Find<T>(id);
            
        }

        public void SaveChanges()
        {
            throw new NotImplementedException();
        }

        public T Update(T entity)
        {
            var updatedEntity = _context.Update(entity).Entity;
            return updatedEntity;
        }
    }
}
