using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.EF.Client.Models.Mapping
{
    public class RegionMap : EntityTypeConfiguration<Region>
    {
        public RegionMap()
        {
            // Primary Key
            this.HasKey(t => t.RegionID);

            // Properties
            this.Property(t => t.RegionDescription)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_RegionDescription", 1) { IsUnique = true })); ;

            // Table & Column Mappings
            this.ToTable("Region");
            this.Property(t => t.RegionID).HasColumnName("RegionID");
            this.Property(t => t.RegionDescription).HasColumnName("RegionDescription");
        }
    }
}
