using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ShareLinkConfiguration : IEntityTypeConfiguration<ShareLink>
    {
        public void Configure(EntityTypeBuilder<ShareLink> b)
        {
            b.ToTable("Mep_hub_ShareLinks");

            b.HasKey(x => x.Id);

            b.Property(x => x.Token)
                .HasMaxLength(64)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.Property(x => x.RevokedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.Token)
                .IsUnique()
                .HasDatabaseName("UX_ShareLink_Token");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
