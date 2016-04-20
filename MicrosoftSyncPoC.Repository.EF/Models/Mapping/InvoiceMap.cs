using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class InvoiceMap : EntityTypeConfiguration<Invoice>
    {
        public InvoiceMap()
        {
            // Primary Key
            HasKey(
                t =>
                    new
                    {
                        t.InvoiceID,
                        t.CustomerID,
                        t.SalespersonID,
                        t.OrderID,
                        t.ShipperID,
                        t.ProductID
                    });

            // Properties
             Property(t => t.InvoiceID).IsRequired();

            Property(t => t.ShipperID).IsRequired();

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

            Property(t => t.CustomerID).IsRequired();
            
            Property(t => t.CustomerName)
                .IsRequired()
                .HasMaxLength(40);

            Property(t => t.Address)
                .HasMaxLength(60);

            Property(t => t.City)
                .HasMaxLength(15);

            Property(t => t.RegionID).IsRequired();

            Property(t => t.PostalCode)
                .HasMaxLength(10);

            Property(t => t.Country)
                .HasMaxLength(15);

            Property(t => t.ShipperID).IsRequired();

            Property(t => t.OrderID).IsRequired();

            Property(t => t.ShipperName)
                .IsRequired()
                .HasMaxLength(40);

            Property(t => t.ProductID).IsRequired();

            Property(t => t.ProductName)
                .IsRequired()
                .HasMaxLength(40);

            Property(t => t.UnitPrice);

            Property(t => t.Quantity);

            // Table & Column Mappings
            ToTable("Invoices");
            Property(t => t.InvoiceID).HasColumnName("InvoiceID");
            Property(t => t.ShipperID).HasColumnName("ShipperID");
            Property(t => t.ShipAddress).HasColumnName("ShipAddress");
            Property(t => t.ShipCity).HasColumnName("ShipCity");
            Property(t => t.ShipRegion).HasColumnName("ShipRegion");
            Property(t => t.ShipPostalCode).HasColumnName("ShipPostalCode");
            Property(t => t.ShipCountry).HasColumnName("ShipCountry");
            Property(t => t.CustomerID).HasColumnName("CustomerID");
            Property(t => t.CustomerName).HasColumnName("CustomerName");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.RegionID).HasColumnName("RegionID");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.Country).HasColumnName("Country");
            Property(t => t.SalespersonID).HasColumnName("Salesperson");
            Property(t => t.OrderID).HasColumnName("OrderID");
            Property(t => t.OrderDate).HasColumnName("OrderDate");
            Property(t => t.RequiredDate).HasColumnName("RequiredDate");
            Property(t => t.ShippedDate).HasColumnName("ShippedDate");
            Property(t => t.ShipperName).HasColumnName("ShipperName");
            Property(t => t.ProductID).HasColumnName("ProductID");
            Property(t => t.ProductName).HasColumnName("ProductName");
            Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            Property(t => t.Quantity).HasColumnName("Quantity");
            Property(t => t.Discount).HasColumnName("Discount");
            Property(t => t.ExtendedPrice).HasColumnName("ExtendedPrice");
            Property(t => t.Freight).HasColumnName("Freight");

            // Relationships
            HasRequired(t => t.Customer).WithMany(t => t.Invoices).HasForeignKey(d => d.CustomerID);
            HasRequired(t => t.Region).WithMany(t => t.Invoices).HasForeignKey(d => d.RegionID);
            HasRequired(t => t.Shipper).WithMany(t => t.Invoices).HasForeignKey(d => d.ShipperID);
            HasRequired(t => t.Order).WithMany(t => t.Invoices).HasForeignKey(d => d.OrderID);
            HasRequired(t => t.Product).WithMany(t => t.Invoices).HasForeignKey(d => d.ProductID);
            HasRequired(t => t.Employee).WithMany(t => t.Invoices).HasForeignKey(d => d.ShipperID);

        }
    }
}