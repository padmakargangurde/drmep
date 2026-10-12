using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects => Set<Project>();
        public DbSet<AppUser> Users => Set<AppUser>();
        public DbSet<ProjectRevision> ProjectRevisions => Set<ProjectRevision>();
        public DbSet<ApprovalCategory> ApprovalCategories => Set<ApprovalCategory>();
        public DbSet<ItemIdentity> ItemIdentities => Set<ItemIdentity>();
        public DbSet<ApprovalItem> ApprovalItems => Set<ApprovalItem>();
        public DbSet<ItemResponse> ItemResponses => Set<ItemResponse>();
        public DbSet<ShareLink> ShareLinks => Set<ShareLink>();
        public DbSet<FinalSubmission> FinalSubmissions => Set<FinalSubmission>();
        public DbSet<AuditLogEntry> AuditLogEntries => Set<AuditLogEntry>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Automatically discovers all IEntityTypeConfiguration<T>
            // classes from this assembly.
            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(AppDbContext).Assembly);
        }
    }
}