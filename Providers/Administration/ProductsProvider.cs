#region Using directives

using System.Data.Entity;
using System.Linq;
using Contracts.Data;
using Data.Entities;

#endregion


namespace Providers.Administration
{
    public class ProductsProvider : EFProvider<Product>, IProductsProvider
    {
        public ProductsProvider(DbContext dbContext) : base(dbContext)
        {
        }

        public IQueryable<Product> GetProductsWithDetails()
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