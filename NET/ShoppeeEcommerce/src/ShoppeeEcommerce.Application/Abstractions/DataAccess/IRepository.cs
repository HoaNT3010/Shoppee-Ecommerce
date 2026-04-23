using Ardalis.Specification;
using ErrorOr;
using ShoppeeEcommerce.Domain.Entities.Base;

namespace ShoppeeEcommerce.Application.Abstractions.DataAccess
{
    public interface IRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
        Task<TEntity?> FirstOrDefaultAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<TResult?> FirstOrDefaultAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
        Task<List<TEntity>> ListAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<List<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> specification, CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
        Task<bool> AnyAsync(ISpecification<TEntity> specification, CancellationToken cancellationToken = default);
    }
}
