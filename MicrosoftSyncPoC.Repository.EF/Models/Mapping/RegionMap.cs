using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;
using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class RegionMap : EntityTypeConfiguration<Region>
    {
        public RegionMap()
        {
            // Primary Key
            HasKey(t => t.RegionID);

            // Properties
            Property(t => t.RegionID)
                ;

            Property(t => t.RegionDescription)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(
                        new IndexAttribute("IX_RegionDescription", 1) { IsUnique = true })); ;

            // Table & Column Mappings
            ToTable("Region");
            Property(t => t.RegionID).HasColumnName("RegionID");
            Property(t => t.RegionDescription).HasColumnName("RegionDescription");
        }
    }
}