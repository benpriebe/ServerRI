#region Using directives

using System.Data.Common;
using System.Data.Entity;
using Data.Entities;
using Data.Entities.Mapping;

#endregion


namespace Data
{
    public class AWContext : DbContext
    {
        static AWContext()
        {
            Database.SetInitializer<AWContext>(null);
        }

        public AWContext() : base("Name=AWContext")
        {
        }

        public AWContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public AWContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection)
        {
        }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<CustomerAddress> CustomerAddresses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductDescription> ProductDescriptions { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ProductTypeProductDescription> ProductTypeProductDescriptions { get; set; }
        public DbSet<SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<SalesOrderHeader> SalesOrderHeaders { get; set; }
        public DbSet<vGetAllCategory> vGetAllCategories { get; set; }
        public DbSet<vProductAndDescription> vProductAndDescriptions { get; set; }
        public DbSet<vProductModelCatalogDescription> vProductModelCatalogDescriptions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AddressMap());
            modelBuilder.Configurations.Add(new CustomerMap());
            modelBuilder.Configurations.Add(new CustomerAddressMap());
            modelBuilder.Configurations.Add(new ProductMap());
            modelBuilder.Configurations.Add(new ProductCategoryMap());
            modelBuilder.Configurations.Add(new ProductDescriptionMap());
            modelBuilder.Configurations.Add(new ProductTypeMap());
            modelBuilder.Configurations.Add(new ProductTypeProductDescriptionMap());
            modelBuilder.Configurations.Add(new SalesOrderDetailMap());
            modelBuilder.Configurations.Add(new SalesOrderHeaderMap());
            modelBuilder.Configurations.Add(new vGetAllCategoryMap());
            modelBuilder.Configurations.Add(new vProductAndDescriptionMap());
            modelBuilder.Configurations.Add(new vProductModelCatalogDescriptionMap());
        }
    }
}