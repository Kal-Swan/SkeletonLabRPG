using System.Linq.Expressions;

namespace SkeletonLabRpg.Common.Database;

public interface IRepository<T> where T : class
{
    Task<T> Create(T model);
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetManyByPredicate(Expression<Func<T, bool>> predicate);

    Task<IEnumerable<T>> GetAll();

    Task<bool> DeleteMany(Expression<Func<T, bool>> predicate);
    Task<T> Update(T entity);
    
    Task<bool> Delete(Guid id);
    
    Task<T> Update(Guid id, T entity);

    Task<T?> GetSingleByPredicate(Expression<Func<T, bool>> predicate);

    Task<T> GetOrCreateAsync(T entity, Expression<Func<T, bool>> predicate);
}