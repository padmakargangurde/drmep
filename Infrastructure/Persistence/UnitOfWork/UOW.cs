using Application.Contracts.Repositories;
using Application.Contracts.UnitOfWork;
using Infrastructure.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUOW
    {
        private readonly AppDbContext _context;

        private IDbContextTransaction? _transaction;

        public IProjectRepository ProjectRepository { get; }

        public IUserRepository UserRepository { get; }

        public IRevisionRepository RevisionRepository { get; }

        public IShareLinkRepository ShareLinkRepository { get; }

        public IClientReviewRepository ClientReviewRepository { get; }

        public IAuditLogRepository AuditLogRepository { get; }

        public IFinalSubmissionRepository FinalSubmissionRepository { get; }

        public UnitOfWork(
            AppDbContext context,
            IProjectRepository projectRepository,
            IUserRepository userRepository,
            IRevisionRepository revisionRepository,
            IShareLinkRepository shareLinkRepository,
            IClientReviewRepository clientReviewRepository,
            IAuditLogRepository auditLogRepository,
            IFinalSubmissionRepository finalSubmissionRepository)
        {
            _context = context;
            ProjectRepository = projectRepository;
            UserRepository = userRepository;
            RevisionRepository = revisionRepository;
            ShareLinkRepository = shareLinkRepository;
            ClientReviewRepository = clientReviewRepository;
            AuditLogRepository = auditLogRepository;
            FinalSubmissionRepository = finalSubmissionRepository;
        }

        /// <summary>
        /// Commits all changes staged in the DbContext.
        /// </summary>
        public async Task<int> SaveChangesAsync(
            CancellationToken ct = default)
        {
            return await _context.SaveChangesAsync(ct);
        }

        /// <summary>
        /// Begins an explicit database transaction.
        /// </summary>
        public async Task BeginTransactionAsync(
            CancellationToken ct = default)
        {
            if (_transaction != null)
            {
                return;
            }

            _transaction = await _context.Database
                .BeginTransactionAsync(ct);
        }

        /// <summary>
        /// Commits the current transaction.
        /// </summary>
        public async Task CommitTransactionAsync(
            CancellationToken ct = default)
        {
            if (_transaction == null)
            {
                return;
            }

            try
            {
                await _transaction.CommitAsync(ct);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        /// <summary>
        /// Rolls back the current transaction.
        /// </summary>
        public async Task RollbackTransactionAsync(
            CancellationToken ct = default)
        {
            if (_transaction == null)
            {
                return;
            }

            try
            {
                await _transaction.RollbackAsync(ct);
            }
            finally
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
}