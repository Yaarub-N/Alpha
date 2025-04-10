using Domain.Responses;
using System.Linq.Expressions;

namespace Data.Interfaces
{
    public interface IBaseRepository<TEntity, TModel> where TEntity : class
    {
        Task<RepositoryResult<bool>> AddAsync(TEntity entity);
        Task<RepositoryResult<bool>> ExistAsync(Expression<Func<TEntity, bool>> findBy);
        Task<RepositoryResult<IEnumerable<TModel>>> GetAllAsync(bool orderByDescending = false, Expression<Func<TEntity, object>>? sortBy = null, Expression<Func<TEntity, bool>>? where = null, Expression<Func<TEntity, TModel>>? select = null, params Expression<Func<TEntity, object>>[] includes);
        Task<RepositoryResult<TModel?>> GetAsync(Expression<Func<TEntity, bool>>? where = null, params Expression<Func<TEntity, object>>[] includes);
        object? GetEntityId(TEntity entity);
        Task<RepositoryResult<bool>> RemoveAsync(TEntity entity);
        Task<RepositoryResult<bool>> UpdateAsync(TEntity entity);
    }
}