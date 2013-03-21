#region Using directives

using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;

#endregion


namespace Providers.Data
{
    /// <summary>
    /// The EF-dependent, generic provider for data access
    /// </summary>
    /// <typeparam name="T">Type of entity for this Repository.</typeparam>
    public class EFProvider<T> : IProvider<T> where T : class
    {
        private readonly OperationContext _context;
        private readonly ILog _log;

        public EFProvider(OperationContext context, ILog log, DbContext dbContext)
        {
            _context = context;
            _log = log;
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public OperationContext Context
        {
            get { return _context; }
        }

        public virtual IQueryable<T> GetAll()
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {
                return DbSet;
            }
        }

        public virtual T GetById(int id)
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with id {0}", id))))
            {
                return DbSet.Find(id);
            }
        }

        public virtual void Add(T entity)
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()))))
            {
                DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
                if (dbEntityEntry.State != EntityState.Detached)
                {
                    dbEntityEntry.State = EntityState.Added;
                }
                else
                {
                    DbSet.Add(entity);
                }
            }
        }

        public virtual void Update(T entity)
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()))))
            {
                DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
                if (dbEntityEntry.State == EntityState.Detached)
                {
                    DbSet.Attach(entity);
                }
                dbEntityEntry.State = EntityState.Modified;
            }
        }

        public virtual void Delete(T entity)
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()))))
            {
                DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
                if (dbEntityEntry.State != EntityState.Deleted)
                {
                    dbEntityEntry.State = EntityState.Deleted;
                }
                else
                {
                    DbSet.Attach(entity);
                    DbSet.Remove(entity);
                }
            }
        }

        public virtual void Delete(int id)
        {
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context, String.Format("with id {0}", id))))
            {
                var entity = GetById(id);
                if (entity == null)
                    throw new NotFoundProviderException(String.Format("Entity not found - {0} - {1}", typeof(T).Name, id));

                Delete(entity);
            }
        }
    }
}