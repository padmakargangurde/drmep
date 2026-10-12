using System.Text.Json;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ApprovalCategoryConfiguration : IEntityTypeConfiguration<ApprovalCategory>
    {
        public void Configure(EntityTypeBuilder<ApprovalCategory> b)
        {
            b.ToTable("Mep_hub_ApprovalCategories");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            b.Property(x => x.SpecReference)
                .HasMaxLength(500);

            var stringListComparer = new ValueComparer<List<string>>(
                (a, c) => (a ?? new List<string>()).SequenceEqual(c ?? new List<string>()),
                v => v.Aggregate(0, (hash, s) => HashCode.Combine(hash, s.GetHashCode())),
                v => v.ToList());

            b.Property(x => x.Columns)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(stringListComparer);

            b.Property(x => x.Warnings)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(stringListComparer);

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasOne(x => x.ProjectRevision)
                .WithMany(x => x.Categories)
                .HasForeignKey(x => x.ProjectRevisionId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasIndex(x => new { x.ProjectRevisionId, x.Name })
                .IsUnique()
                .HasDatabaseName("UX_ApprovalCategory_Name");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
