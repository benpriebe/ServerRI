#region Using directives

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class vProductAndDescriptionMap : EntityTypeConfiguration<vProductAndDescription>
    {
        public vProductAndDescriptionMap()
        {
            // Primary Key
            HasKey(t => new
            {
                t.ProductID, t.Name, t.ProductModel, t.Culture, t.Description
            });

            // Properties
            Property(t => t.ProductID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.ProductModel)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Culture)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(6);

            Property(t => t.Description)
                .IsRequired()
                .HasMaxLength(400);

            // Table & Column Mappings
            ToTable("vProductAndDescription", "SalesLT");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.ProductModel).HasColumnName("ProductModel");
            Property(t => t.Culture).HasColumnName("Culture");
            Property(t => t.Description).HasColumnName("Description");
        }
    }
}