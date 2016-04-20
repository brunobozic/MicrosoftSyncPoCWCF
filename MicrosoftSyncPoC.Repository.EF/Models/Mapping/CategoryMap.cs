using System.Data.Entity.ModelConfiguration;

namespace MicrosoftSyncPoC.Repository.EF.Models.Mapping
{
    public class CategoryMap : EntityTypeConfiguration<Category>
    {
        public CategoryMap()
        {
            // Primary Key
            HasKey(t => t.CategoryID);

            // Properties
            Property(t => t.CategoryName)
                .IsRequired()
                .HasMaxLength(35);

            // Table & Column Mappings
            ToTable("Categories");
            Property(t => t.CategoryID).HasColumnName("CategoryID");
            Property(t => t.CategoryName).HasColumnName("CategoryName");
            Property(t => t.Description).HasColumnName("Description");
            Property(t => t.Picture).HasColumnName("Picture");
        }
    }
}