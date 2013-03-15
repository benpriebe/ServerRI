#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class ProductCategoryMap : EntityTypeConfiguration<ProductCategory>
    {
        public ProductCategoryMap()
        {
            // Primary Key
            HasKey(t => t.ProductCategoryID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("ProductCategory", "SalesLT");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.ParentProductCategoryID).HasColumnName("ParentProductCategoryID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasOptional(t => t.ProductCategory2)
                .WithMany(t => t.ProductCategory1)
                .HasForeignKey(d => d.ParentProductCategoryID);
        }
    }
}