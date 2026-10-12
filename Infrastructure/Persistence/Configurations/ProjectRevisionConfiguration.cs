using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProjectRevisionConfiguration : IEntityTypeConfiguration<ProjectRevision>
    {
        public void Configure(EntityTypeBuilder<ProjectRevision> b)
        {
            b.ToTable("Mep_hub_ProjectRevisions");

            b.HasKey(x => x.Id);

            b.Property(x => x.Label)
                .HasMaxLength(100)
                .IsRequired();

            b.Property(x => x.SourceFileName)
                .HasMaxLength(260)
                .IsRequired();

            b.Property(x => x.UploadedAt)
                .HasColumnType("datetime2(3)");

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.UploadedBy)
                .WithMany()
                .HasForeignKey(x => x.UploadedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Exactly one active revision per project.
            b.HasIndex(x => x.ProjectId)
                .IsUnique()
                .HasFilter("[IsActive] = 1")
                .HasDatabaseName("UX_ProjectRevision_ActivePerProject");

            b.HasIndex(x => new { x.ProjectId, x.RevisionNumber })
                .IsUnique()
                .HasDatabaseName("UX_ProjectRevision_Number");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
