using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.EF.Client.Models.Mapping
{
    public class ShipperMap : EntityTypeConfiguration<Shipper>
    {
        public ShipperMap()
        {
            // Primary Key
            HasKey(t => t.ShipperID);

            // Properties
            Property(t => t.CompanyName)
                .IsRequired()
                .HasMaxLength(40)
                .HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_CompanyName", 1) {IsUnique = true}));


            Property(t => t.Phone)
                .HasMaxLength(35);

            // Table & Column Mappings
            ToTable("Shippers");
            Property(t => t.ShipperID).HasColumnName("ShipperID");
            Property(t => t.CompanyName).HasColumnName("CompanyName");
            Property(t => t.Phone).HasColumnName("Phone");
        }
    }
}