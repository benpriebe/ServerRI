#region Using directives

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class vProductModelCatalogDescriptionMap : EntityTypeConfiguration<vProductModelCatalogDescription>
    {
        public vProductModelCatalogDescriptionMap()
        {
            // Primary Key
            HasKey(t => new
            {
                t.ProductModelID, t.Name, t.rowguid, t.ModifiedDate
            });

            // Properties
            Property(t => t.ProductModelID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Copyright)
                .HasMaxLength(30);

            Property(t => t.ProductURL)
                .HasMaxLength(256);

            Property(t => t.WarrantyPeriod)
                .HasMaxLength(256);

            Property(t => t.WarrantyDescription)
                .HasMaxLength(256);

            Property(t => t.NoOfYears)
                .HasMaxLength(256);

            Property(t => t.MaintenanceDescription)
                .HasMaxLength(256);

            Property(t => t.Wheel)
                .HasMaxLength(256);

            Property(t => t.Saddle)
                .HasMaxLength(256);

            Property(t => t.Pedal)
                .HasMaxLength(256);

            Property(t => t.Crankset)
                .HasMaxLength(256);

            Property(t => t.PictureAngle)
                .HasMaxLength(256);

            Property(t => t.PictureSize)
                .HasMaxLength(256);

            Property(t => t.ProductPhotoID)
                .HasMaxLength(256);

            Property(t => t.Material)
                .HasMaxLength(256);

            Property(t => t.Color)
                .HasMaxLength(256);

            Property(t => t.ProductLine)
                .HasMaxLength(256);

            Property(t => t.Style)
                .HasMaxLength(256);

            Property(t => t.RiderExperience)
                .HasMaxLength(1024);

            // Table & Column Mappings
            ToTable("vProductModelCatalogDescription", "SalesLT");
            Property(t => t.ProductModelID).HasColumnName("ProductModelID");
            Property(t => t.Name).HasColumnName("Name");
            Property(t => t.Summary).HasColumnName("Summary");
            Property(t => t.Manufacturer).HasColumnName("Manufacturer");
            Property(t => t.Copyright).HasColumnName("Copyright");
            Property(t => t.ProductURL).HasColumnName("ProductURL");
            Property(t => t.WarrantyPeriod).HasColumnName("WarrantyPeriod");
            Property(t => t.WarrantyDescription).HasColumnName("WarrantyDescription");
            Property(t => t.NoOfYears).HasColumnName("NoOfYears");
            Property(t => t.MaintenanceDescription).HasColumnName("MaintenanceDescription");
            Property(t => t.Wheel).HasColumnName("Wheel");
            Property(t => t.Saddle).HasColumnName("Saddle");
            Property(t => t.Pedal).HasColumnName("Pedal");
            Property(t => t.BikeFrame).HasColumnName("BikeFrame");
            Property(t => t.Crankset).HasColumnName("Crankset");
            Property(t => t.PictureAngle).HasColumnName("PictureAngle");
            Property(t => t.PictureSize).HasColumnName("PictureSize");
            Property(t => t.ProductPhotoID).HasColumnName("ProductPhotoID");
            Property(t => t.Material).HasColumnName("Material");
            Property(t => t.Color).HasColumnName("Color");
            Property(t => t.ProductLine).HasColumnName("ProductLine");
            Property(t => t.Style).HasColumnName("Style");
            Property(t => t.RiderExperience).HasColumnName("RiderExperience");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}