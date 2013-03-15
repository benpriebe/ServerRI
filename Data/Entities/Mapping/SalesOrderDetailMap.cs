#region Using directives

using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

#endregion


namespace Data.Entities.Mapping
{
    public class SalesOrderDetailMap : EntityTypeConfiguration<SalesOrderDetail>
    {
        public SalesOrderDetailMap()
        {
            // Primary Key
            HasKey(t => new
            {
                t.SalesOrderID, t.SalesOrderDetailID
            });

            // Properties
            Property(t => t.SalesOrderID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.SalesOrderDetailID)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            // Table & Column Mappings
            ToTable("SalesOrderDetail", "SalesLT");
            Property(t => t.SalesOrderID).HasColumnName("SalesOrderID");
            Property(t => t.SalesOrderDetailID).HasColumnName("SalesOrderDetailID");
            Property(t => t.OrderQty).HasColumnName("OrderQty");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            Property(t => t.UnitPriceDiscount).HasColumnName("UnitPriceDiscount");
            Property(t => t.LineTotal).HasColumnName("LineTotal");
            Property(t => t.rowguid).HasColumnName("rowguid");
            Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");

            // Relationships
            HasRequired(t => t.Product)
                .WithMany(t => t.SalesOrderDetails)
                .HasForeignKey(d => d.ProductID);
            HasRequired(t => t.SalesOrderHeader)
                .WithMany(t => t.SalesOrderDetails)
                .HasForeignKey(d => d.SalesOrderID);
        }
    }
}