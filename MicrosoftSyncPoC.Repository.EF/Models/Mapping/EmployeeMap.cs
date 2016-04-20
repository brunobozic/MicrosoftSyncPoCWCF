using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class EmployeeMap : EntityTypeConfiguration<Employee>
    {
        public EmployeeMap()
        {
            // Primary Key
            HasKey(t => t.EmployeeID);

            // Properties
            Property(t => t.LastName)
                .IsRequired()
                .HasMaxLength(35);

            Property(t => t.FirstName)
                .IsRequired()
                .HasMaxLength(35);

            Property(t => t.Title)
                .HasMaxLength(35);

            Property(t => t.TitleOfCourtesy)
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

            Property(t => t.HomePhone)
                .HasMaxLength(35);

            Property(t => t.Extension)
                .HasMaxLength(35);

            Property(t => t.PhotoPath)
                .HasMaxLength(255);

            // Table & Column Mappings
            ToTable("Employees");
            Property(t => t.EmployeeID).HasColumnName("EmployeeID");
            Property(t => t.LastName).HasColumnName("LastName");
            Property(t => t.FirstName).HasColumnName("FirstName");
            Property(t => t.Title).HasColumnName("Title");
            Property(t => t.TitleOfCourtesy).HasColumnName("TitleOfCourtesy");
            Property(t => t.BirthDate).HasColumnName("BirthDate");
            Property(t => t.HireDate).HasColumnName("HireDate");
            Property(t => t.Address).HasColumnName("Address");
            Property(t => t.City).HasColumnName("City");
            Property(t => t.Region).HasColumnName("Region");
            Property(t => t.PostalCode).HasColumnName("PostalCode");
            Property(t => t.Country).HasColumnName("Country");
            Property(t => t.HomePhone).HasColumnName("HomePhone");
            Property(t => t.Extension).HasColumnName("Extension");
            Property(t => t.Photo).HasColumnName("Photo");
            Property(t => t.Notes).HasColumnName("Notes");
            Property(t => t.ReportsTo).HasColumnName("ReportsTo");
            Property(t => t.PhotoPath).HasColumnName("PhotoPath");

            // Relationships
        

            HasOptional(t => t.Employee1)
                .WithMany(t => t.Employees1)
                .HasForeignKey(d => d.ReportsTo);
        }
    }
}