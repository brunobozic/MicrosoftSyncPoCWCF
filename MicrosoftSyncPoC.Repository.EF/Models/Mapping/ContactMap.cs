using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class ContactMap : EntityTypeConfiguration<Contact>
    {
        public ContactMap()
        {
            // Primary Key
            HasKey(t => t.ContactID);

            // Properties
            Property(t => t.ContactType)
                .HasMaxLength(50);

            Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(40);

            Property(t => t.ContactName)
                .HasMaxLength(35);

            Property(t => t.ContactTitle)
                .HasMaxLength(35);

            Property(t => t.Address)
                .HasMaxLength(60);

            Property(t => t.City)
                .HasMaxLength(35);

            Property(t => t.Region)
                .HasMaxLength(35);

            Property(t => t.PostalCode)
                .HasMaxLength(35);

            Property(t => t.Country)
                .HasMaxLength(35);

            Property(t => t.Phone)
                .HasMaxLength(35);

            Property(t => t.Extension)
                .HasMaxLength(35);

            Property(t => t.Fax)
                .HasMaxLength(35);

            Property(t => t.PhotoPath)
                .HasMaxLength(255);

            // Table & Column Mappings
            ToTable("Contacts");
            Property(t => t.ContactID).HasColumnName("ContactID");
            Property(t => t.ContactType).HasColumnName("ContactType");
            Property(t => t.CompanyName).HasColumnName("CompanyName");
            Property(t => t.ContactName).HasColumnName("ContactName");
            Property(t => t.ContactTitle).HasColumnName("ContactTitle");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.Region).HasColumnName("Region");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.Country).HasColumnName("Country");
            Property(t => t.Phone).HasColumnName("Phone");
            Property(t => t.Extension).HasColumnName("Extension");
            Property(t => t.Fax).HasColumnName("Fax");
            Property(t => t.HomePage).HasColumnName("HomePage");
            Property(t => t.PhotoPath).HasColumnName("PhotoPath");
            Property(t => t.Photo).HasColumnName("Photo");
        }
    }
}