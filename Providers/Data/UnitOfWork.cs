#region Using directives

using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using Contracts.Data;
using Data.Entities;

#endregion


namespace Providers.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _context;
        private readonly IProvider<Address> _addressesProvider;
        private readonly IProductsProvider _productsProvider;
        private readonly IProvider<Customer> _customersProvider;
        private IProvider<SalesOrderDetail> _orderDetailsProvider;
        private IProvider<ProductCategory> _productCategoriesProvider;

        public UnitOfWork(DbContext context, 
            Func<DbContext, IProductsProvider> productsProvider, 
            Func<DbContext, IProvider<Customer>> customersProvider,
            Func<DbContext, IProvider<Address>> addressesProvider,
            Func<DbContext, IProvider<SalesOrderDetail>> orderDetailsProvider,
            Func<DbContext, IProvider<ProductCategory>> productCategoriesProvider)
        {
            _context = context;
            _addressesProvider = addressesProvider(context);
            _productsProvider = productsProvider(context);
            _customersProvider = customersProvider(context);
            _orderDetailsProvider = orderDetailsProvider(context);
            _productCategoriesProvider = productCategoriesProvider(context);

            InitDbContext();
        }

        public IProductsProvider Products
        {
            get { return _productsProvider; }
        }

        public IProvider<SalesOrderDetail> OrderDetails
        {
            get { return _orderDetailsProvider; }
        }
        
        public IProvider<ProductCategory> ProductCategories
        {
            get { return _productCategoriesProvider; }
        }

        public IProvider<Customer> Customers
        {
            get { return _customersProvider; }
        }

        public IProvider<Address> Addresses
        {
            get { return _addressesProvider; }
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
            DbContext.Configuration.ValidateOnSaveEnabled = false; //TODO: 03-Apr-2013 - Ben - Re-assess.

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