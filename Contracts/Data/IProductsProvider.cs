#region Using directives

using System.Linq;
using Data.Entities;

#endregion


namespace Contracts.Data
{
    public interface IProductsProvider : IProvider<Product>
    {
        IQueryable<Product> GetProductsWithDetails();
    }
}