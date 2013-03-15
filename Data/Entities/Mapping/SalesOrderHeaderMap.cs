#region Using directives

using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class SalesOrderHeaderMap : EntityTypeConfiguration<SalesOrderHeader>
    {
        public SalesOrderHeaderMap()
        {
            // Primary Key
            HasKey(t => t.SalesOrderID);

            // Properties
            Property(t => t.SalesOrderNumber)
                .IsRequired()
                .HasMaxLength(25);

            Property(t => t.PurchaseOrderNumber)
                .HasMaxLength(25);

            Property(t => t.AccountNumber)
                .HasMaxLength(15);

            Property(t => t.ShipMethod)
                .IsRequired()
                .HasMaxLength(50);

            Property(t => t.CreditCardApprovalCode)
                .HasMaxLength(15);

            // Table & Column Mappings
            ToTable("SalesOrderHeader", "SalesLT");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.RevisionNumber).HasColumnName("RevisionNumber");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.DueDate).HasColumnName("DueDate");
            Property(t => t.ShipDate).HasColumnName("ShipDate");
            Property(t => t.Status).HasColumnName("Status");
            Property(t => t.OnlineOrderFlag).HasColumnName("OnlineOrderFlag");
            Property(t => t.SalesOrderNumber).HasColumnName("SalesOrderNumber");
            Property(t => t.PurchaseOrderNumber).HasColumnName("PurchaseOrderNumber");
            Property(t => t.AccountNumber).HasColumnName("AccountNumber");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.ShipToAddressID).HasColumnName("ShipToAddressID");
            Property(t => t.BillToAddressID).HasColumnName("BillToAddressID");
            Property(t => t.ShipMethod).HasColumnName("ShipMethod");
            Property(t => t.CreditCardApprovalCode).HasColumnName("CreditCardApprovalCode");
            Property(t => t.SubTotal).HasColumnName("SubTotal");
            Property(t => t.TaxAmt).HasColumnName("TaxAmt");
            Property(t => t.Freight).HasColumnName("Freight");
            Property(t => t.TotalDue).HasColumnName("TotalDue");
            Property(t => t.Comment).HasColumnName("Comment");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasOptional(t => t.Address)
                .WithMany(t => t.SalesOrderHeaders)
                .HasForeignKey(d => d.BillToAddressID);
            HasOptional(t => t.Address1)
                .WithMany(t => t.SalesOrderHeaders1)
                .HasForeignKey(d => d.ShipToAddressID);
            HasRequired(t => t.Customer)
                .WithMany(t => t.SalesOrderHeaders)
                .HasForeignKey(d => d.CustomerID);
        }
    }
}