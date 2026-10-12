using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ItemResponseConfiguration : IEntityTypeConfiguration<ItemResponse>
    {
        public void Configure(EntityTypeBuilder<ItemResponse> b)
        {
            b.ToTable("Mep_hub_ItemResponses");

            b.HasKey(x => x.Id);

            b.Property(x => x.Decision)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            b.Property(x => x.ReviewedByName)
                .HasMaxLength(150);

            b.Property(x => x.RespondedAt)
                .HasColumnType("datetime2(3)");

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasIndex(x => x.ItemIdentityId)
                .IsUnique()
                .HasDatabaseName("UX_ItemResponse_ItemIdentity");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
