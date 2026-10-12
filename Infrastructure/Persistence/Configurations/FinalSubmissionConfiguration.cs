using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class FinalSubmissionConfiguration : IEntityTypeConfiguration<FinalSubmission>
    {
        public void Configure(EntityTypeBuilder<FinalSubmission> b)
        {
            b.ToTable("Mep_hub_FinalSubmissions");

            b.HasKey(x => x.Id);

            b.Property(x => x.SubmittedByName)
                .HasMaxLength(150)
                .IsRequired();

            b.Property(x => x.SubmittedByCompany)
                .HasMaxLength(200);

            b.Property(x => x.SubmittedAt)
                .HasColumnType("datetime2(3)");

            b.Property(x => x.ReopenedAt)
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

            b.HasOne(x => x.ProjectRevision)
                .WithMany()
                .HasForeignKey(x => x.ProjectRevisionId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ReopenedBy)
                .WithMany()
                .HasForeignKey(x => x.ReopenedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ProjectId, x.SubmittedAt })
                .HasDatabaseName("IX_FinalSubmission_Project");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
