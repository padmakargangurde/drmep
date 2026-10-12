using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> b)
        {
            b.ToTable("Mep_hub_Projects");

            b.HasKey(x => x.Id);

            b.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            b.Property(x => x.ProjectNumber)
                .HasMaxLength(50)
                .IsRequired();

            b.Property(x => x.Name)
                .HasMaxLength(250)
                .IsRequired();

            b.Property(x => x.ClientName)
                .HasMaxLength(200)
                .IsRequired();

            b.Property(x => x.ClientCompany)
                .HasMaxLength(200);

            b.Property(x => x.Location)
                .HasMaxLength(200);

            b.Property(x => x.Description);

            b.Property(x => x.SiteContext)
                .HasColumnType("varchar(30)");

            b.Property(x => x.IsPublished)
                .HasDefaultValue(false);

            b.Property(x => x.IsDeleted)
                .HasDefaultValue(false);

            b.Property(x => x.CreatedAt)
                .HasColumnType("datetime2(3)")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.Property(x => x.UpdatedAt)
                .HasColumnType("datetime2(3)");

            // JSON column
            b.OwnsOne(x => x.ImportSettings, s =>
            {
                s.ToJson("ImportSettingsJson");

                s.Property(p => p.IgnoredSheets)
                    .HasJsonPropertyName("ignoredSheets");

                s.Property(p => p.IgnoredColumns)
                    .HasJsonPropertyName("ignoredColumns");
            });

            // User references — Restrict, not Cascade: SQL Server rejects multiple
            // cascade paths from Project to AppUser (PM, BIM coordinator, creator).
            b.HasOne(x => x.ProjectManager)
                .WithMany()
                .HasForeignKey(x => x.ProjectManagerUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.BimCoordinator)
                .WithMany()
                .HasForeignKey(x => x.BimCoordinatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.CreatedBy)
                .WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.CreatedByUserId)
                .HasFilter("[CreatedByUserId] IS NOT NULL")
                .HasDatabaseName("IX_Project_CreatedByUserId");

            // Unique project number among non-deleted projects
            b.HasIndex(x => x.ProjectNumber)
                .IsUnique()
                .HasFilter("[IsDeleted] = 0")
                .HasDatabaseName("UX_Project_Number");

            // Dashboard list index
            b.HasIndex(x => new
            {
                x.IsDeleted,
                x.CreatedAt
            })
            .IncludeProperties(x => new
            {
                x.Name,
                x.ProjectNumber,
                x.ClientName,
                x.IsPublished
            })
            .HasDatabaseName("IX_Project_List");

            // Soft delete filter
            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}