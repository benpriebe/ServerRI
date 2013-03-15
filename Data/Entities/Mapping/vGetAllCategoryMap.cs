#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class vGetAllCategoryMap : EntityTypeConfiguration<vGetAllCategory>
    {
        public vGetAllCategoryMap()
        {
            // Primary Key
            HasKey(t => t.ParentProductCategoryName);

            // Properties
            Property(t => t.ParentProductCategoryName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ProductCategoryName)
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("vGetAllCategories", "SalesLT");
            Property(t => t.ParentProductCategoryName).HasColumnName("ParentProductCategoryName");
            Property(t => t.ProductCategoryName).HasColumnName("ProductCategoryName");
            Property(t => t.ProductCategoryID).HasColumnName("ProductCategoryID");
        }
    }
}