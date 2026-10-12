using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AuditLogEntryConfiguration : IEntityTypeConfiguration<AuditLogEntry>
    {
        public void Configure(EntityTypeBuilder<AuditLogEntry> b)
        {
            b.ToTable("Mep_hub_AuditLogEntries");

            b.HasKey(x => x.Id);

            b.Property(x => x.Timestamp)
                .HasColumnType("datetime2(3)");

            b.Property(x => x.ActorDisplayName)
                .HasMaxLength(150)
                .IsRequired();

            b.Property(x => x.Action)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Actor)
                .WithMany()
                .HasForeignKey(x => x.ActorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ProjectId, x.Timestamp })
                .HasDatabaseName("IX_AuditLogEntry_Project");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
