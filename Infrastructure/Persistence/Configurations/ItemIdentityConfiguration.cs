using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ItemIdentityConfiguration : IEntityTypeConfiguration<ItemIdentity>
    {
        public void Configure(EntityTypeBuilder<ItemIdentity> b)
        {
            b.ToTable("Mep_hub_ItemIdentities");

            b.HasKey(x => x.Id);

            b.Property(x => x.IdentityKey)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.CategoryName)
                .HasMaxLength(150)
                .IsRequired();

            b.Property(x => x.Section)
                .HasMaxLength(250);

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Response)
                .WithOne(x => x.ItemIdentity)
                .HasForeignKey<ItemResponse>(x => x.ItemIdentityId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ProjectId, x.IdentityKey })
                .IsUnique()
                .HasDatabaseName("UX_ItemIdentity_Key");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
