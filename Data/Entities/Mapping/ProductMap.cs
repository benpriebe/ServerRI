#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class ProductMap : EntityTypeConfiguration<Product>
    {
        public ProductMap()
        {
            // Primary Key
            HasKey(t => t.ProductID);

            // Properties
            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ProductNumber)
                .IsRequired()
                .HasMaxLength(25);

            Property(t => t.Color)
                .HasMaxLength(15);

            Property(t => t.Size)
                .HasMaxLength(5);

            Property(t => t.ThumbnailPhotoFileName)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Product", "SalesLT");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ProductNumber).HasColumnName("ProductNumber");
            Property(t => t.Color).HasColumnName("Color");
            Property(t => t.StandardCost).HasColumnName("StandardCost");
            Property(t => t.ListPrice).HasColumnName("ListPrice");
            Property(t => t.Size).HasColumnName("Size");
            Property(t => t.Weight).HasColumnName("Weight");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
            Property(t => t.ProductTypeID).HasColumnName("ProductModelID");
            Property(t => t.SellStartDate).HasColumnName("SellStartDate");
            Property(t => t.SellEndDate).HasColumnName("SellEndDate");
            Property(t => t.DiscontinuedDate).HasColumnName("DiscontinuedDate");
            Property(t => t.ThumbNailPhoto).HasColumnName("ThumbNailPhoto");
            Property(t => t.ThumbnailPhotoFileName).HasColumnName("ThumbnailPhotoFileName");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasOptional(t => t.ProductCategory)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.ProductCategoryID);
            HasOptional(t => t.ProductType)
                .WithMany(t => t.Products)
                .HasForeignKey(d => d.ProductTypeID);
        }
    }
}