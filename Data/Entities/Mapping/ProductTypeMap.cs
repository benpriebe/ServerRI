#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class ProductTypeMap : EntityTypeConfiguration<ProductType>
    {
        public ProductTypeMap()
        {
            // Primary Key
            HasKey(t => t.ProductTypeID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductModel", "SalesLT");
            Property(t => t.ProductTypeID).HasColumnName("ProductModelID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.CatalogDescription).HasColumnName("CatalogDescription");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}