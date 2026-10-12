using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Base
{
   
    /// <summary>
    /// Non-generic marker for any persisted entity.
    ///
    /// Only useful if you add a generic repository or a SaveChanges
    /// interceptor - both need to reference an entity without knowing its key
    /// type, which IEntity&lt;TKey&gt; cannot do. If neither of those is
    /// planned, this interface has no callers and can be deleted.
    /// </summary>
    public interface IBase
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
        bool IsDeleted { get; set; }
    }

    /// <summary>
    /// Shared shape for persisted entities: identity plus audit and
    /// soft-delete columns.
    /// </summary>
    public interface IEntity<TKey> : IBase
    {
        TKey Id { get; set; }
    }

    /// <summary>
    /// Base class carrying the four columns every entity needs, so each entity
    /// declares only its own fields.
    ///
    /// This is the one that pays off: ProjectRevision, ApprovalCategory,
    /// ItemIdentity and ApprovalItem are all still to come, and inheriting
    /// removes four property declarations from each.
    ///
    /// Note IsDeleted is inherited by everything, including the revision
    /// snapshot tables where it will always be false - categories and items
    /// are hard-deleted along with their revision. Harmless, but if you would
    /// rather not carry a dead column there, drop IsDeleted from here and put
    /// it on an ISoftDeletable applied only to Project.
    /// </summary>
    public abstract class BaseEntity<TKey> : IEntity<TKey>
    {
        /// <summary>Database-assigned identity. Do not set before insert.</summary>
        public TKey Id { get; set; } = default!;

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}
