#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class AddressMap : EntityTypeConfiguration<Address>
    {
        public AddressMap()
        {
            // Primary Key
            HasKey(t => t.AddressID);

            // Properties
            Property(t => t.AddressLine1)
                .IsRequired()
                .HasMaxLength(60);

            Property(t => t.AddressLine2)
                .HasMaxLength(60);

            Property(t => t.City)
                .IsRequired()
                .HasMaxLength(30);

            Property(t => t.StateProvince)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CountryRegion)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.PostalCode)
                .IsRequired()
                .HasMaxLength(15);

            // Table & Column Mappings
            ToTable("Address", "SalesLT");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressLine1).HasColumnName("AddressLine1");
            Property(t => t.AddressLine2).HasColumnName("AddressLine2");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.StateProvince).HasColumnName("StateProvince");
            Property(t => t.CountryRegion).HasColumnName("CountryRegion");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}