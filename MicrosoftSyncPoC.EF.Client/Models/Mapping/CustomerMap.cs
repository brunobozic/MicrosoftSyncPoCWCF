using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.EF.Client.Models.Mapping
{
    public class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            // Primary Key
            this.HasKey(t => t.CustomerID);

            // Properties
            this.Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(40);

            this.Property(t => t.ContactName)
                .HasMaxLength(35);

            this.Property(t => t.ContactTitle)
                .HasMaxLength(35);

            this.Property(t => t.Address)
                .HasMaxLength(60);

            this.Property(t => t.City)
                .HasMaxLength(35);

            this.Property(t => t.Region)
                .HasMaxLength(35);

            this.Property(t => t.PostalCode)
                .HasMaxLength(35);

            this.Property(t => t.Country)
                .HasMaxLength(35);

            this.Property(t => t.Phone)
                .HasMaxLength(35);

            this.Property(t => t.Fax)
                .HasMaxLength(35);

            // Table & Column Mappings
            this.ToTable("Customers");
            this.Property(t => t.CustomerID).HasColumnName("CustomerID");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.ContactName).HasColumnName("ContactName");
            this.Property(t => t.ContactTitle).HasColumnName("ContactTitle");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Region).HasColumnName("Region");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Fax).HasColumnName("Fax");
        }
    }
}
