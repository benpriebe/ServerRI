#region Using directives

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class ProductTypeProductDescriptionMap : EntityTypeConfiguration<ProductTypeProductDescription>
    {
        public ProductTypeProductDescriptionMap()
        {
            // Primary Key
            HasKey(t => new
            {
                t.ProductTypeID, t.ProductDescriptionID, t.Culture
            });

            // Properties
            Property(t => t.ProductTypeID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.ProductDescriptionID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Culture)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            // Table & Column Mappings
            ToTable("ProductModelProductDescription", "SalesLT");
            Property(t => t.ProductTypeID).HasColumnName("ProductModelID");
            Property(t => t.ProductDescriptionID).HasColumnName("ProductDescriptionID");
            Property(t => t.Culture).HasColumnName("Culture");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.ProductDescription)
                .WithMany(t => t.ProductTypeProductDescriptions)
                .HasForeignKey(d => d.ProductDescriptionID);
            HasRequired(t => t.ProductType)
                .WithMany(t => t.ProductTypeProductDescriptions)
                .HasForeignKey(d => d.ProductTypeID);
        }
    }
}