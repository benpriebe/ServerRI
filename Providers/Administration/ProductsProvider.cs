#region Using directives

using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Contracts.Data;
using Core;
using Core.Extensions;
using Data.Entities;

#endregion


namespace Providers.Administration
{
    public class ProductsProvider : EFProvider<Product>, IProductsProvider
    {
        private readonly ILog _log;

        public ProductsProvider(OperationContext context, ILog log, DbContext dbContext) : base(context, log, dbContext)
        {
            _log = log;
        }

        public IQueryable<Product> GetProductsWithDetails()
        {
            _log.Enter(GetType(), MethodBase.GetCurrentMethod(), Context);

            var products = DbContext
                .Set<Product>()
                .Include(p => p.ProductType)
                .Include(p => p.ProductCategory)
                .AsNoTracking();

            _log.Exit(GetType(), MethodBase.GetCurrentMethod(), Context);
            return products;
        }
    }
}