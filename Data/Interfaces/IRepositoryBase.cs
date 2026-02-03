using System.Linq.Expressions;

namespace HondaSensorChecker.Data.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        // CREATE
        bool Add(T entity, out string error);

        // READ
        IEnumerable<T> GetAll();
        T GetById(int id);
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        // UPDATE
        bool Edit(T entity, out string error);

        // DELETE
        bool Remove(int id, out string error);
    }
}