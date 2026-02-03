using HondaSensorChecker.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq.Expressions;

namespace HondaSensorChecker.Data.Repositories
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public RepositoryBase(DbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        // ============================
        // CREATE
        // ============================
        public virtual bool Add(T entity, out string error)
        {
            error = string.Empty;

            try
            {
                _dbSet.Add(entity);
                return true;
            }
            catch (Exception ex)
            {
                error = ExtractMessage(ex);
                return false;
            }
        }

        // ============================
        // DELETE
        // ============================
        public virtual bool Remove(int id, out string error)
        {
            error = string.Empty;

            try
            {
                // Find the entity to remove
                var entity = GetById(id);
                if (entity == null)
                {
                    error = "Registro não encontrado.";
                    return false;
                }

                // Ensure we are not tracking another instance with the same key
                DetachLocalIfTracked(entity);

                _dbSet.Remove(entity);
                return true;
            }
            catch (DbUpdateException)
            {
                // This is typically a foreign key restriction at commit time
                error = "Registro possui vínculos e não pode ser removido.";
                return false;
            }
            catch (Exception ex)
            {
                error = ExtractMessage(ex);
                return false;
            }
        }

        // ============================
        // READ
        // ============================
        public virtual IEnumerable<T> GetAll()
        {
            // NoTracking is good for grids and read-only scenarios
            return _dbSet.AsNoTracking().ToList();
        }

        public virtual T GetById(int id)
        {
            // Find may return a tracked entity if it is already in the context
            return _dbSet.Find(id);
        }

        public virtual IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.AsNoTracking().Where(predicate).ToList();
        }

        // ============================
        // UPDATE
        // ============================
        public virtual bool Edit(T entity, out string error)
        {
            error = string.Empty;

            try
            {
                // If another instance with the same PK is already tracked,
                // we must detach it before attaching this one.
                DetachLocalIfTracked(entity);

                // Attach and mark as modified (safer than Update for disconnected entities)
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;

                return true;
            }
            catch (Exception ex)
            {
                error = ExtractMessage(ex);
                return false;
            }
        }

        // ============================
        // Helpers
        // ============================
        protected string ExtractMessage(Exception ex)
        {
            return ex.InnerException?.Message ?? ex.Message;
        }

        private void DetachLocalIfTracked(T entity)
        {
            // If EF is already tracking an entity with the same key,
            // it will throw when we try to attach another instance.
            var primaryKey = GetPrimaryKey();
            if (primaryKey == null || primaryKey.Properties.Count == 0)
                return;

            // This repository assumes a single-column primary key (your models use that pattern)
            var keyProperty = primaryKey.Properties[0];
            var keyValue = GetKeyValue(entity, keyProperty);
            if (keyValue == null)
                return;

            var trackedEntity = _dbSet.Local.FirstOrDefault(local =>
            {
                var localKeyValue = GetKeyValue(local, keyProperty);
                return localKeyValue != null && localKeyValue.Equals(keyValue);
            });

            if (trackedEntity != null)
            {
                _dbContext.Entry(trackedEntity).State = EntityState.Detached;
            }
        }

        private IKey? GetPrimaryKey()
        {
            return _dbContext.Model.FindEntityType(typeof(T))?.FindPrimaryKey();
        }

        private object? GetKeyValue(object obj, IProperty keyProperty)
        {
            var propertyInfo = obj.GetType().GetProperty(keyProperty.Name);
            return propertyInfo?.GetValue(obj);
        }
    }
}
