using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class TerritoryMap : EntityTypeConfiguration<Territory>
    {
        public TerritoryMap()
        {
            // Primary Key
            HasKey(t => t.TerritoryID);

            // Properties
            Property(t => t.TerritoryID)
                .IsRequired();


            Property(t => t.TerritoryDescription)
                .IsRequired()
                .IsFixedLength()
                .HasMaxLength(50);

            // Table & Column Mappings
            ToTable("Territories");
            Property(t => t.TerritoryID).HasColumnName("TerritoryID");
            Property(t => t.TerritoryDescription).HasColumnName("TerritoryDescription");
            Property(t => t.RegionID).HasColumnName("RegionID");

            // Relationships
            HasRequired(t => t.Region)
                .WithMany(t => t.Territories)
                .HasForeignKey(d => d.RegionID);
        }
    }
}