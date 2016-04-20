using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.EF.Client.Models.Mapping
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            // Primary Key
            this.HasKey(t => t.ContactID);

            // Properties
            this.Property(t => t.ContactType)
                .HasMaxLength(50);

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

            this.Property(t => t.Extension)
                .HasMaxLength(35);

            this.Property(t => t.Fax)
                .HasMaxLength(35);

            this.Property(t => t.PhotoPath)
                .HasMaxLength(255);

            // Table & Column Mappings
            this.ToTable("Contacts");
            this.Property(t => t.ContactID).HasColumnName("ContactID");
            this.Property(t => t.ContactType).HasColumnName("ContactType");
            this.Property(t => t.CompanyName).HasColumnName("CompanyName");
            this.Property(t => t.ContactName).HasColumnName("ContactName");
            this.Property(t => t.ContactTitle).HasColumnName("ContactTitle");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.Region).HasColumnName("Region");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.Country).HasColumnName("Country");
            this.Property(t => t.Phone).HasColumnName("Phone");
            this.Property(t => t.Extension).HasColumnName("Extension");
            this.Property(t => t.Fax).HasColumnName("Fax");
            this.Property(t => t.HomePage).HasColumnName("HomePage");
            this.Property(t => t.PhotoPath).HasColumnName("PhotoPath");
            this.Property(t => t.Photo).HasColumnName("Photo");
        }
    }
}
