using Application.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Contracts.UnitOfWork
{
    public   interface IUOW
    {

        IProjectRepository ProjectRepository { get; }

        IUserRepository UserRepository { get; }

        IRevisionRepository RevisionRepository { get; }

        IShareLinkRepository ShareLinkRepository { get; }

        IClientReviewRepository ClientReviewRepository { get; }

        IAuditLogRepository AuditLogRepository { get; }

        IFinalSubmissionRepository FinalSubmissionRepository { get; }

        /// <summary>Commits everything staged since the last call.</summary>
        Task<int> SaveChangesAsync(CancellationToken ct = default);

        /// <summary>
        /// Explicit transaction, for writes that span several repositories and
        /// need to roll back together — the import commit being the case this
        /// exists for.
        /// </summary>
        Task BeginTransactionAsync(CancellationToken ct = default);

        Task CommitTransactionAsync(CancellationToken ct = default);

        Task RollbackTransactionAsync(CancellationToken ct = default);
    }
}
