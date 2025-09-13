using Microsoft.EntityFrameworkCore;
using Application.Interfaces;
using System.Linq.Expressions;

namespace Infrastructure.Database
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _set;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public Task<List<T>> GetAllAsync()
        {
            return _set.AsNoTracking().ToListAsync();
        }

        public Task<T?> GetByIdAsync(object id)
        {
            return _set.FindAsync([id]).AsTask();
        }

        public Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return  _set.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _set.AddAsync(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<T> UpdateAsync(T entity)
        {
            var entry = _context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                _set.Attach(entity);
            }

            entry.State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(object id)
        {
            var entity = await _set.FindAsync([id]);

            if (entity is null) return false;

            _set.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
