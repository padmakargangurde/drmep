using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> b)
        {
            b.ToTable("Mep_hub_Users");

            b.HasKey(x => x.Id);

            b.Property(x => x.Name)
                .HasMaxLength(150)
                .IsRequired();

            b.Property(x => x.Email)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.Role)
                .HasConversion<string>()
                .HasMaxLength(30)
                .IsRequired();

            b.Property(x => x.IsActive)
                .HasDefaultValue(true);

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            b.HasIndex(x => x.Email)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("UX_AppUser_Email");

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
