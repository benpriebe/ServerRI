#region Using directives

using System;
using System.Data.Entity;
using Contracts.Data;
using Data.Entities;

#endregion


namespace Providers
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IProductsProvider _productsProvider;
        private readonly IProvider<Customer> _customersProvider;

        public UnitOfWork(DbContext context, Func<DbContext, IProductsProvider> productsProvider, Func<DbContext, IProvider<Customer>> customersProvider)
        {
            _context = context;
            _productsProvider = productsProvider(context);
            _customersProvider = customersProvider(context);
            InitDbContext();
        }

        public IProductsProvider Products
        {
            get { return _productsProvider; }
        }

        public IProvider<Customer> Customers
        {
            get { return _customersProvider; }
        }

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            try
            {
                DbContext.SaveChanges();
            }
            catch (Exception e)
            {
                throw new EFProviderException("The provider cannot complete the operation. See error collection for more details", e);
            }
        }

        protected void InitDbContext()
        {
            DbContext = _context;

            // Do NOT enable proxied entities, else serialization fails
            DbContext.Configuration.ProxyCreationEnabled = false;

            // Load navigation properties explicitly (avoid serialization trouble)
            DbContext.Configuration.LazyLoadingEnabled = false;

            // Because Web API will perform validation, we don't need/want EF to do so
            DbContext.Configuration.ValidateOnSaveEnabled = false;

            //DbContext.Configuration.AutoDetectChangesEnabled = false;
            // We won't use this performance tweak because we don't need 
            // the extra performance and, when autodetect is false,
            // we'd have to be careful. We're not being that careful.
        }

        private DbContext DbContext { get; set; }


        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion
    }
}