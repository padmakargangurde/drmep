/* ============================================================================
   MEP Approval Hub — Production Schema (Mep_hub_ prefixed tables)
   Target: SQL Server
   Matches the current EF Core model (Fields column already removed from
   ApprovalItem — Excel row data lives only in MongoDB, referenced here via
   FieldsDocumentId). Run this once against a fresh production database.
   ============================================================================ */

SET NOCOUNT ON;
GO

/* ----------------------------------------------------------------------------
   1. Mep_hub_Users  (was: AppUser)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_Users
(
    Id          INT IDENTITY(1,1)   NOT NULL,
    Name        NVARCHAR(150)       NOT NULL,
    Email       NVARCHAR(200)       NOT NULL,
    Role        NVARCHAR(30)        NOT NULL,   -- Admin | ProjectManager | BimCoordinator | Client
    IsActive    BIT                 NOT NULL CONSTRAINT DF_Mep_hub_Users_IsActive DEFAULT (1),
    IsDeleted   BIT                 NOT NULL CONSTRAINT DF_Mep_hub_Users_IsDeleted DEFAULT (0),
    CreatedAt   DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_Users_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt   DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_Users PRIMARY KEY CLUSTERED (Id)
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_Users_Email
    ON dbo.Mep_hub_Users (Email)
    WHERE IsDeleted = 0;
GO

/* ----------------------------------------------------------------------------
   2. Mep_hub_Projects  (was: Project)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_Projects
(
    Id                      INT IDENTITY(1,1)   NOT NULL,
    ProjectNumber           NVARCHAR(50)        NOT NULL,
    Name                    NVARCHAR(250)       NOT NULL,
    ClientName              NVARCHAR(200)       NOT NULL,
    ClientCompany           NVARCHAR(200)       NULL,
    Location                NVARCHAR(200)       NULL,
    Description             NVARCHAR(MAX)       NULL,
    DueDate                 DATE                NULL,
    SiteContext             VARCHAR(30)         NULL,
    WorkspaceId             UNIQUEIDENTIFIER    NULL,
    ProjectManagerUserId    INT                 NULL,
    BimCoordinatorUserId    INT                 NULL,
    CreatedByUserId         INT                 NULL,
    IsPublished             BIT                 NOT NULL CONSTRAINT DF_Mep_hub_Projects_IsPublished DEFAULT (0),
    IsDeleted               BIT                 NOT NULL CONSTRAINT DF_Mep_hub_Projects_IsDeleted DEFAULT (0),
    CreatedAt               DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_Projects_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt               DATETIME2(3)        NULL,
    ImportSettingsJson      NVARCHAR(MAX)       NULL,   -- owned-entity JSON column (ignoredSheets/ignoredColumns)
    CONSTRAINT PK_Mep_hub_Projects PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_Projects_ProjectManager FOREIGN KEY (ProjectManagerUserId) REFERENCES dbo.Mep_hub_Users (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Mep_hub_Projects_BimCoordinator  FOREIGN KEY (BimCoordinatorUserId) REFERENCES dbo.Mep_hub_Users (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Mep_hub_Projects_CreatedBy       FOREIGN KEY (CreatedByUserId)      REFERENCES dbo.Mep_hub_Users (Id) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_Projects_Number
    ON dbo.Mep_hub_Projects (ProjectNumber)
    WHERE IsDeleted = 0;

CREATE INDEX IX_Mep_hub_Projects_CreatedByUserId
    ON dbo.Mep_hub_Projects (CreatedByUserId)
    WHERE CreatedByUserId IS NOT NULL;

CREATE INDEX IX_Mep_hub_Projects_ProjectManagerUserId
    ON dbo.Mep_hub_Projects (ProjectManagerUserId);

CREATE INDEX IX_Mep_hub_Projects_BimCoordinatorUserId
    ON dbo.Mep_hub_Projects (BimCoordinatorUserId);

CREATE INDEX IX_Mep_hub_Projects_List
    ON dbo.Mep_hub_Projects (IsDeleted, CreatedAt)
    INCLUDE (Name, ProjectNumber, ClientName, IsPublished);
GO

/* ----------------------------------------------------------------------------
   3. Mep_hub_ProjectRevisions  (was: ProjectRevision)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ProjectRevisions
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProjectId           INT                 NOT NULL,
    RevisionNumber      INT                 NOT NULL,
    Label               NVARCHAR(100)       NOT NULL,
    SourceFileName      NVARCHAR(260)       NOT NULL,
    IsActive            BIT                 NOT NULL,
    UploadedAt          DATETIME2(3)        NOT NULL,
    UploadedByUserId    INT                 NULL,
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ProjectRevisions_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ProjectRevisions_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ProjectRevisions PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ProjectRevisions_Project    FOREIGN KEY (ProjectId)        REFERENCES dbo.Mep_hub_Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Mep_hub_ProjectRevisions_UploadedBy FOREIGN KEY (UploadedByUserId) REFERENCES dbo.Mep_hub_Users (Id)    ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_ProjectRevisions_ActivePerProject
    ON dbo.Mep_hub_ProjectRevisions (ProjectId)
    WHERE IsActive = 1;

CREATE UNIQUE INDEX UX_Mep_hub_ProjectRevisions_Number
    ON dbo.Mep_hub_ProjectRevisions (ProjectId, RevisionNumber);

CREATE INDEX IX_Mep_hub_ProjectRevisions_UploadedByUserId
    ON dbo.Mep_hub_ProjectRevisions (UploadedByUserId);
GO

/* ----------------------------------------------------------------------------
   4. Mep_hub_ApprovalCategories  (was: ApprovalCategory)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ApprovalCategories
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProjectRevisionId   INT                 NOT NULL,
    Name                NVARCHAR(150)       NOT NULL,
    Columns             NVARCHAR(MAX)       NOT NULL,   -- JSON array of Excel column headers
    SpecReference       NVARCHAR(500)       NULL,
    Warnings            NVARCHAR(MAX)       NOT NULL,   -- JSON array of parse warnings
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ApprovalCategories_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ApprovalCategories_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ApprovalCategories PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ApprovalCategories_ProjectRevision FOREIGN KEY (ProjectRevisionId) REFERENCES dbo.Mep_hub_ProjectRevisions (Id) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_ApprovalCategories_Name
    ON dbo.Mep_hub_ApprovalCategories (ProjectRevisionId, Name);
GO

/* ----------------------------------------------------------------------------
   5. Mep_hub_ItemIdentities  (was: ItemIdentity)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ItemIdentities
(
    Id              INT IDENTITY(1,1)   NOT NULL,
    ProjectId       INT                 NOT NULL,
    IdentityKey     NVARCHAR(64)        NOT NULL,   -- stable hash identifying this row across revisions
    CategoryName    NVARCHAR(150)       NOT NULL,
    Section         NVARCHAR(250)       NULL,
    IsDeleted       BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ItemIdentities_IsDeleted DEFAULT (0),
    CreatedAt       DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ItemIdentities_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ItemIdentities PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ItemIdentities_Project FOREIGN KEY (ProjectId) REFERENCES dbo.Mep_hub_Projects (Id) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_ItemIdentities_Key
    ON dbo.Mep_hub_ItemIdentities (ProjectId, IdentityKey);
GO

/* ----------------------------------------------------------------------------
   6. Mep_hub_ApprovalItems  (was: ApprovalItem)
      NOTE: no Fields column — per-row Excel field data lives only in MongoDB;
      FieldsDocumentId points at that Mongo document's _id.
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ApprovalItems
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ItemIdentityId      INT                 NOT NULL,
    ProjectRevisionId   INT                 NOT NULL,
    ApprovalCategoryId  INT                 NOT NULL,
    PreviousItemId      INT                 NULL,       -- self-reference: prior revision's snapshot of this same item
    ItemNo              INT                 NOT NULL,
    Section             NVARCHAR(250)       NULL,
    SourceStatus        NVARCHAR(100)       NULL,
    SpecReference       NVARCHAR(500)       NULL,
    LowConfidence       BIT                 NOT NULL,
    ChangeType          NVARCHAR(20)        NOT NULL,   -- Added | Modified | Unchanged | Removed
    FieldsDocumentId    NVARCHAR(50)        NULL,       -- MongoDB _id in mep_excelsheet_records
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ApprovalItems_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ApprovalItems_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ApprovalItems PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ApprovalItems_ApprovalCategory FOREIGN KEY (ApprovalCategoryId) REFERENCES dbo.Mep_hub_ApprovalCategories (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Mep_hub_ApprovalItems_ItemIdentity     FOREIGN KEY (ItemIdentityId)     REFERENCES dbo.Mep_hub_ItemIdentities (Id)     ON DELETE NO ACTION,
    CONSTRAINT FK_Mep_hub_ApprovalItems_ProjectRevision  FOREIGN KEY (ProjectRevisionId)  REFERENCES dbo.Mep_hub_ProjectRevisions (Id)   ON DELETE CASCADE,
    CONSTRAINT FK_Mep_hub_ApprovalItems_PreviousItem     FOREIGN KEY (PreviousItemId)     REFERENCES dbo.Mep_hub_ApprovalItems (Id)      ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Mep_hub_ApprovalItems_ApprovalCategoryId
    ON dbo.Mep_hub_ApprovalItems (ApprovalCategoryId);

CREATE INDEX IX_Mep_hub_ApprovalItems_ItemIdentityId
    ON dbo.Mep_hub_ApprovalItems (ItemIdentityId);

CREATE INDEX IX_Mep_hub_ApprovalItems_PreviousItemId
    ON dbo.Mep_hub_ApprovalItems (PreviousItemId);

CREATE INDEX IX_Mep_hub_ApprovalItems_CategoryOrder
    ON dbo.Mep_hub_ApprovalItems (ProjectRevisionId, ApprovalCategoryId, ItemNo);
GO

/* ----------------------------------------------------------------------------
   7. Mep_hub_ItemResponses  (was: ItemResponse)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ItemResponses
(
    Id              INT IDENTITY(1,1)   NOT NULL,
    ItemIdentityId  INT                 NOT NULL,
    Decision        NVARCHAR(20)        NOT NULL,   -- Pending | Approved | Rejected | Clarify | NA
    Comment         NVARCHAR(MAX)       NULL,
    ReviewedByName  NVARCHAR(150)       NULL,
    RespondedAt     DATETIME2(3)        NULL,
    IsDeleted       BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ItemResponses_IsDeleted DEFAULT (0),
    CreatedAt       DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ItemResponses_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt       DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ItemResponses PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ItemResponses_ItemIdentity FOREIGN KEY (ItemIdentityId) REFERENCES dbo.Mep_hub_ItemIdentities (Id) ON DELETE CASCADE
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_ItemResponses_ItemIdentity
    ON dbo.Mep_hub_ItemResponses (ItemIdentityId);
GO

/* ----------------------------------------------------------------------------
   8. Mep_hub_ShareLinks  (was: ShareLink)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_ShareLinks
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProjectId           INT                 NOT NULL,
    Token               NVARCHAR(64)        NOT NULL,
    IsRevoked           BIT                 NOT NULL,
    RevokedAt           DATETIME2(3)        NULL,
    CreatedByUserId     INT                 NULL,
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_ShareLinks_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_ShareLinks_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_ShareLinks PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_ShareLinks_Project   FOREIGN KEY (ProjectId)       REFERENCES dbo.Mep_hub_Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Mep_hub_ShareLinks_CreatedBy FOREIGN KEY (CreatedByUserId) REFERENCES dbo.Mep_hub_Users (Id)    ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX UX_Mep_hub_ShareLinks_Token
    ON dbo.Mep_hub_ShareLinks (Token);

CREATE INDEX IX_Mep_hub_ShareLinks_ProjectId
    ON dbo.Mep_hub_ShareLinks (ProjectId);

CREATE INDEX IX_Mep_hub_ShareLinks_CreatedByUserId
    ON dbo.Mep_hub_ShareLinks (CreatedByUserId);
GO

/* ----------------------------------------------------------------------------
   9. Mep_hub_FinalSubmissions  (was: FinalSubmission)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_FinalSubmissions
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProjectId           INT                 NOT NULL,
    ProjectRevisionId   INT                 NOT NULL,
    SubmittedByName     NVARCHAR(150)       NOT NULL,
    SubmittedByCompany  NVARCHAR(200)       NULL,
    SubmittedAt         DATETIME2(3)        NOT NULL,
    ApprovedCount       INT                 NOT NULL,
    RejectedCount       INT                 NOT NULL,
    CommentedCount      INT                 NOT NULL,
    ClarifyCount        INT                 NOT NULL,
    NaCount             INT                 NOT NULL,
    ReopenedAt          DATETIME2(3)        NULL,
    ReopenedByUserId    INT                 NULL,
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_FinalSubmissions_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_FinalSubmissions_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_FinalSubmissions PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_FinalSubmissions_Project         FOREIGN KEY (ProjectId)         REFERENCES dbo.Mep_hub_Projects (Id)         ON DELETE CASCADE,
    CONSTRAINT FK_Mep_hub_FinalSubmissions_ProjectRevision FOREIGN KEY (ProjectRevisionId) REFERENCES dbo.Mep_hub_ProjectRevisions (Id) ON DELETE NO ACTION,
    CONSTRAINT FK_Mep_hub_FinalSubmissions_ReopenedBy      FOREIGN KEY (ReopenedByUserId)  REFERENCES dbo.Mep_hub_Users (Id)            ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Mep_hub_FinalSubmissions_ProjectRevisionId
    ON dbo.Mep_hub_FinalSubmissions (ProjectRevisionId);

CREATE INDEX IX_Mep_hub_FinalSubmissions_ReopenedByUserId
    ON dbo.Mep_hub_FinalSubmissions (ReopenedByUserId);

CREATE INDEX IX_Mep_hub_FinalSubmissions_Project
    ON dbo.Mep_hub_FinalSubmissions (ProjectId, SubmittedAt);
GO

/* ----------------------------------------------------------------------------
   10. Mep_hub_AuditLogEntries  (was: AuditLogEntry)
   ---------------------------------------------------------------------------- */
CREATE TABLE dbo.Mep_hub_AuditLogEntries
(
    Id                  INT IDENTITY(1,1)   NOT NULL,
    ProjectId           INT                 NOT NULL,
    ActorUserId         INT                 NULL,
    ActorDisplayName    NVARCHAR(150)       NOT NULL,
    Action              NVARCHAR(100)       NOT NULL,
    Detail              NVARCHAR(MAX)       NULL,
    Timestamp           DATETIME2(3)        NOT NULL,
    IsDeleted           BIT                 NOT NULL CONSTRAINT DF_Mep_hub_AuditLogEntries_IsDeleted DEFAULT (0),
    CreatedAt           DATETIME2(3)        NOT NULL CONSTRAINT DF_Mep_hub_AuditLogEntries_CreatedAt DEFAULT (SYSUTCDATETIME()),
    UpdatedAt           DATETIME2(3)        NULL,
    CONSTRAINT PK_Mep_hub_AuditLogEntries PRIMARY KEY CLUSTERED (Id),
    CONSTRAINT FK_Mep_hub_AuditLogEntries_Project FOREIGN KEY (ProjectId)   REFERENCES dbo.Mep_hub_Projects (Id) ON DELETE CASCADE,
    CONSTRAINT FK_Mep_hub_AuditLogEntries_Actor   FOREIGN KEY (ActorUserId) REFERENCES dbo.Mep_hub_Users (Id)    ON DELETE NO ACTION
);
GO

CREATE INDEX IX_Mep_hub_AuditLogEntries_ActorUserId
    ON dbo.Mep_hub_AuditLogEntries (ActorUserId);

CREATE INDEX IX_Mep_hub_AuditLogEntries_Project
    ON dbo.Mep_hub_AuditLogEntries (ProjectId, Timestamp);
GO

/* ============================================================================
   End of script — 10 tables created:
   Mep_hub_Users, Mep_hub_Projects, Mep_hub_ProjectRevisions,
   Mep_hub_ApprovalCategories, Mep_hub_ItemIdentities, Mep_hub_ApprovalItems,
   Mep_hub_ItemResponses, Mep_hub_ShareLinks, Mep_hub_FinalSubmissions,
   Mep_hub_AuditLogEntries
   ============================================================================ */
