using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            // Primary Key
            HasKey(t => t.OrderID);

            // Properties
            Property(t => t.CustomerID);

            Property(t => t.ShipName)
                .HasMaxLength(40);

            Property(t => t.ShipAddress)
                .HasMaxLength(60);

            Property(t => t.ShipCity)
                .HasMaxLength(35);

            Property(t => t.ShipRegion)
                .HasMaxLength(35);

            Property(t => t.ShipPostalCode)
                .HasMaxLength(35);

            Property(t => t.ShipCountry)
                .HasMaxLength(35);

            // Table & Column Mappings
            ToTable("Orders");
            Property(t => t.OrderID).HasColumnName("OrderID");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.RequiredDate).HasColumnName("RequiredDate");
            Property(t => t.ShippedDate).HasColumnName("ShippedDate");
            Property(t => t.ShipVia).HasColumnName("ShipVia");
            Property(t => t.Freight).HasColumnName("Freight");
            Property(t => t.ShipName).HasColumnName("ShipName");
            Property(t => t.ShipAddress).HasColumnName("ShipAddress");
            Property(t => t.ShipCity).HasColumnName("ShipCity");
            Property(t => t.ShipRegion).HasColumnName("ShipRegion");
            Property(t => t.ShipPostalCode).HasColumnName("ShipPostalCode");
            Property(t => t.ShipCountry).HasColumnName("ShipCountry");

            // Relationships
            HasOptional(t => t.Customer)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.CustomerID);
            HasOptional(t => t.Employee)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.EmployeeID);
            HasOptional(t => t.Shipper)
                .WithMany(t => t.Orders)
                .HasForeignKey(d => d.ShipVia);
        }
    }
}