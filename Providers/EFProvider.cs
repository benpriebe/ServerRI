#region Using directives

using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Contracts.Data;
using Core.Extensions;

#endregion


namespace Providers
{
    /// <summary>
    /// The EF-dependent, generic provider for data access
    /// </summary>
    /// <typeparam name="T">Type of entity for this Repository.</typeparam>
    public class EFProvider<T> : IProvider<T> where T : class
    {
        private readonly ILog _log;

        public EFProvider(ILog log, DbContext dbContext)
        {
            _log = log;
            if (dbContext == null)
                throw new ArgumentNullException("dbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod());
            return DbSet;
        }

        public virtual T GetById(int id)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod());
            return DbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
        }

        public virtual void Update(T entity)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }
            dbEntityEntry.State = EntityState.Modified;
            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
        }

        public virtual void Delete(T entity)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
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
            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with entity {0} - {1}", typeof(T).Name, entity.ToJson()));
        }

        public virtual void Delete(int id)
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), String.Format("with id {0}", id));
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), String.Format("with id {0}", id));
        }
    }
}