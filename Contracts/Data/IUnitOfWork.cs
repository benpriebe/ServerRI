#region Using directives

using System;
using Data.Entities;

#endregion


namespace Contracts.Data
{
    public interface IUnitOfWork : IDisposable
    {
        void Commit();

        IProductsProvider Products { get; }
        IProvider<Customer> Customers { get; }
        IProvider<SalesOrderDetail> OrderDetails { get; }
        IProvider<ProductCategory> ProductCategories { get; }
        IProvider<Address> Addresses { get; }
    }
}