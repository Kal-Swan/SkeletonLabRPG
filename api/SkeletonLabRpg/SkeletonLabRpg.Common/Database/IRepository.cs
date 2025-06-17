using System.Linq.Expressions;

namespace SkeletonLabRpg.Common.Database;

public interface IRepository<T> where T : class
{
    Task<T> Create(T model);
    Task<T?> GetById(Guid id);
    Task<IEnumerable<T>> GetMany(Expression<Func<T, bool>> predicate);

    Task<bool> DeleteMany(Expression<Func<T, bool>> predicate);
    Task<T> Update(T entity);
    
    Task<bool> Delete(Guid id);
    
    Task<T> Update(Guid id, T entity);

    Task<T?> GetByPredicate(Expression<Func<T, bool>> predicate);
}