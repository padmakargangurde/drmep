using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ApprovalItemConfiguration : IEntityTypeConfiguration<ApprovalItem>
    {
        public void Configure(EntityTypeBuilder<ApprovalItem> b)
        {
            b.ToTable("Mep_hub_ApprovalItems");

            b.HasKey(x => x.Id);

            b.Property(x => x.Section)
                .HasMaxLength(250);

            b.Property(x => x.SourceStatus)
                .HasMaxLength(100);

            b.Property(x => x.SpecReference)
                .HasMaxLength(500);

            // Fields (the Excel row's cell values) lives entirely in MongoDB now — SQL only
            // keeps the pointer to that document.
            b.Ignore(x => x.Fields);

            b.Property(x => x.FieldsDocumentId)
                .HasMaxLength(50);

            b.Property(x => x.ChangeType)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.ProjectRevision)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ProjectRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.ApprovalCategory)
                .WithMany()
                .HasForeignKey(x => x.ApprovalCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.ItemIdentity)
                .WithMany(x => x.Snapshots)
                .HasForeignKey(x => x.ItemIdentityId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.PreviousItem)
                .WithMany()
                .HasForeignKey(x => x.PreviousItemId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => new { x.ProjectRevisionId, x.ApprovalCategoryId, x.ItemNo })
                .HasDatabaseName("IX_ApprovalItem_CategoryOrder");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
