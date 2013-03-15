#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            HasKey(t => t.CustomerID);

            // Properties
            Property(t => t.Title)
                .HasMaxLength(8);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.MiddleName)
                .HasMaxLength(50);

            Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.Suffix)
                .HasMaxLength(10);

            Property(t => t.CompanyName)
                .HasMaxLength(128);

            Property(t => t.SalesPerson)
                .HasMaxLength(256);

            Property(t => t.EmailAddress)
                .HasMaxLength(50);

            Property(t => t.Phone)
                .HasMaxLength(25);

            Property(t => t.PasswordHash)
                .IsRequired()
                .HasMaxLength(128);

            Property(t => t.PasswordSalt)
                .IsRequired()
                .HasMaxLength(10);

            // Table & Column Mappings
            ToTable("Customer", "SalesLT");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.NameStyle).HasColumnName("NameStyle");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.MiddleName).HasColumnName("MiddleName");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.Suffix).HasColumnName("Suffix");
            Property(t => t.CompanyName).HasColumnName("CompanyName");
            Property(t => t.SalesPerson).HasColumnName("SalesPerson");
            Property(t => t.EmailAddress).HasColumnName("EmailAddress");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            Property(t => t.PasswordSalt).HasColumnName("PasswordSalt");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        }
    }
}