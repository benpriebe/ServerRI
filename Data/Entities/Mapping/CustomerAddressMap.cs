#region Using directives

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class CustomerAddressMap : EntityTypeConfiguration<CustomerAddress>
    {
        public CustomerAddressMap()
        {
            // Primary Key
            HasKey(t => new
            {
                t.CustomerID, t.AddressID
            });

            // Properties
            Property(t => t.CustomerID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.AddressType)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("CustomerAddress", "SalesLT");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.AddressID).HasColumnName("AddressID");
            Property(t => t.AddressType).HasColumnName("AddressType");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Address)
                .WithMany(t => t.CustomerAddresses)
                .HasForeignKey(d => d.AddressID);
            HasRequired(t => t.Customer)
                .WithMany(t => t.CustomerAddresses)
                .HasForeignKey(d => d.CustomerID);
        }
    }
}