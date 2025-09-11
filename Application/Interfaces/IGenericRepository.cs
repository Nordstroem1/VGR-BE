using Domain.Models;

namespace Application.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<OperationResult<IEnumerable<T>>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<OperationResult<T>> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task<OperationResult<T>> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<OperationResult<T>> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<OperationResult<T>> DeleteAsync(object id, CancellationToken cancellationToken = default);
    }
}
