using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Application.Interfaces;

namespace Infrastructure.Database
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _set;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _set = _context.Set<T>();
        }

        public async Task<OperationResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var items = await _set.AsNoTracking().ToListAsync(cancellationToken);
                return OperationResult<IEnumerable<T>>.SuccessResult(items);
            }
            catch (Exception ex)
            {
                return OperationResult<IEnumerable<T>>.FailureResult($"Failed to retrieve {typeof(T).Name} items: {ex.Message}");
            }
        }

        public async Task<OperationResult<T>> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _set.FindAsync(new object?[] { id }, cancellationToken);
                if (entity is null)
                {
                    return OperationResult<T>.FailureResult($"{typeof(T).Name} with id '{id}' was not found.");
                }
                return OperationResult<T>.SuccessResult(entity);
            }
            catch (Exception ex)
            {
                return OperationResult<T>.FailureResult($"Failed to retrieve {typeof(T).Name} with id '{id}': {ex.Message}");
            }
        }

        public async Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
            {
                return OperationResult<T>.FailureResult("Entity cannot be null.");
            }

            try
            {
                await _set.AddAsync(entity, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<T>.SuccessResult(entity);
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<T>.FailureResult($"Database update error while adding {typeof(T).Name}: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return OperationResult<T>.FailureResult($"Unexpected error while adding {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity is null)
            {
                return OperationResult<T>.FailureResult("Entity cannot be null.");
            }

            try
            {
                var entry = _context.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    _set.Attach(entity);
                }
                entry.State = EntityState.Modified;

                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<T>.SuccessResult(entity);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return OperationResult<T>.FailureResult($"Concurrency error while updating {typeof(T).Name}: {ex.Message}");
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<T>.FailureResult($"Database update error while updating {typeof(T).Name}: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return OperationResult<T>.FailureResult($"Unexpected error while updating {typeof(T).Name}: {ex.Message}");
            }
        }

        public async Task<OperationResult<T>> DeleteAsync(object id, CancellationToken cancellationToken = default)
        {
            try
            {
                var entity = await _set.FindAsync(new object?[] { id }, cancellationToken);
                if (entity is null)
                {
                    return OperationResult<T>.FailureResult($"{typeof(T).Name} with id '{id}' was not found.");
                }

                _set.Remove(entity);
                await _context.SaveChangesAsync(cancellationToken);
                return OperationResult<T>.SuccessResult(entity);
            }
            catch (DbUpdateException dbEx)
            {
                return OperationResult<T>.FailureResult($"Database update error while deleting {typeof(T).Name} with id '{id}': {dbEx.Message}");
            }
            catch (Exception ex)
            {
                return OperationResult<T>.FailureResult($"Unexpected error while deleting {typeof(T).Name} with id '{id}': {ex.Message}");
            }
        }
    }
}
