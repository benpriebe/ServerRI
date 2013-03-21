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


namespace Providers.Data
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
            using (new OperationLogger(_log, m => m.Invoke(GetType(), MethodBase.GetCurrentMethod(), Context)))
            {

                var products = DbContext
                    .Set<Product>()
                    .Include(p => p.ProductType)
                    .Include(p => p.ProductCategory)
                    .AsNoTracking();
             
                return products;
            }
        }
    }
}