using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalWorkflowSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BimCoordinator",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedByName",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectManager",
                table: "Project");

            // AlterColumn can't cast uniqueidentifier -> int (SQL Server rejects the
            // implicit conversion even on an empty table), so drop and re-add instead.
            // The filtered index on this column has to go first — SQL Server won't
            // drop a column an index depends on.
            migrationBuilder.DropIndex(
                name: "IX_Project_CreatedByUserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Project");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserId",
                table: "Project",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedByUserId",
                table: "Project",
                column: "CreatedByUserId",
                filter: "[CreatedByUserId] IS NOT NULL");

            migrationBuilder.AddColumn<int>(
                name: "BimCoordinatorUserId",
                table: "Project",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProjectManagerUserId",
                table: "Project",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemIdentity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    IdentityKey = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Section = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemIdentity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemIdentity_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogEntry",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ActorUserId = table.Column<int>(type: "int", nullable: true),
                    ActorDisplayName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogEntry_AppUser_ActorUserId",
                        column: x => x.ActorUserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuditLogEntry_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRevision",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    RevisionNumber = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    UploadedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRevision", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRevision_AppUser_UploadedByUserId",
                        column: x => x.UploadedByUserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectRevision_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShareLink",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    CreatedByUserId = table.Column<int>(type: "int", nullable: true),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    RevokedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShareLink", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShareLink_AppUser_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShareLink_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemResponse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ItemIdentityId = table.Column<int>(type: "int", nullable: false),
                    Decision = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    RespondedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemResponse", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemResponse_ItemIdentity_ItemIdentityId",
                        column: x => x.ItemIdentityId,
                        principalTable: "ItemIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalCategory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectRevisionId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Columns = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Warnings = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalCategory_ProjectRevision_ProjectRevisionId",
                        column: x => x.ProjectRevisionId,
                        principalTable: "ProjectRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinalSubmission",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    ProjectRevisionId = table.Column<int>(type: "int", nullable: false),
                    SubmittedByName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SubmittedByCompany = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false),
                    ApprovedCount = table.Column<int>(type: "int", nullable: false),
                    RejectedCount = table.Column<int>(type: "int", nullable: false),
                    ClarifyCount = table.Column<int>(type: "int", nullable: false),
                    NaCount = table.Column<int>(type: "int", nullable: false),
                    CommentedCount = table.Column<int>(type: "int", nullable: false),
                    ReopenedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    ReopenedByUserId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinalSubmission", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinalSubmission_AppUser_ReopenedByUserId",
                        column: x => x.ReopenedByUserId,
                        principalTable: "AppUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinalSubmission_ProjectRevision_ProjectRevisionId",
                        column: x => x.ProjectRevisionId,
                        principalTable: "ProjectRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinalSubmission_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectRevisionId = table.Column<int>(type: "int", nullable: false),
                    ApprovalCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemIdentityId = table.Column<int>(type: "int", nullable: false),
                    ItemNo = table.Column<int>(type: "int", nullable: false),
                    Section = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Fields = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceStatus = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpecReference = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    LowConfidence = table.Column<bool>(type: "bit", nullable: false),
                    ChangeType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PreviousItemId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2(3)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalItem_ApprovalCategory_ApprovalCategoryId",
                        column: x => x.ApprovalCategoryId,
                        principalTable: "ApprovalCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalItem_ApprovalItem_PreviousItemId",
                        column: x => x.PreviousItemId,
                        principalTable: "ApprovalItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalItem_ItemIdentity_ItemIdentityId",
                        column: x => x.ItemIdentityId,
                        principalTable: "ItemIdentity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalItem_ProjectRevision_ProjectRevisionId",
                        column: x => x.ProjectRevisionId,
                        principalTable: "ProjectRevision",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Project_BimCoordinatorUserId",
                table: "Project",
                column: "BimCoordinatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectManagerUserId",
                table: "Project",
                column: "ProjectManagerUserId");

            migrationBuilder.CreateIndex(
                name: "UX_ApprovalCategory_Name",
                table: "ApprovalCategory",
                columns: new[] { "ProjectRevisionId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalItem_ApprovalCategoryId",
                table: "ApprovalItem",
                column: "ApprovalCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalItem_CategoryOrder",
                table: "ApprovalItem",
                columns: new[] { "ProjectRevisionId", "ApprovalCategoryId", "ItemNo" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalItem_ItemIdentityId",
                table: "ApprovalItem",
                column: "ItemIdentityId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalItem_PreviousItemId",
                table: "ApprovalItem",
                column: "PreviousItemId");

            migrationBuilder.CreateIndex(
                name: "UX_AppUser_Email",
                table: "AppUser",
                column: "Email",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntry_ActorUserId",
                table: "AuditLogEntry",
                column: "ActorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogEntry_Project",
                table: "AuditLogEntry",
                columns: new[] { "ProjectId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_FinalSubmission_Project",
                table: "FinalSubmission",
                columns: new[] { "ProjectId", "SubmittedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FinalSubmission_ProjectRevisionId",
                table: "FinalSubmission",
                column: "ProjectRevisionId");

            migrationBuilder.CreateIndex(
                name: "IX_FinalSubmission_ReopenedByUserId",
                table: "FinalSubmission",
                column: "ReopenedByUserId");

            migrationBuilder.CreateIndex(
                name: "UX_ItemIdentity_Key",
                table: "ItemIdentity",
                columns: new[] { "ProjectId", "IdentityKey" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UX_ItemResponse_ItemIdentity",
                table: "ItemResponse",
                column: "ItemIdentityId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRevision_UploadedByUserId",
                table: "ProjectRevision",
                column: "UploadedByUserId");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectRevision_ActivePerProject",
                table: "ProjectRevision",
                column: "ProjectId",
                unique: true,
                filter: "[IsActive] = 1");

            migrationBuilder.CreateIndex(
                name: "UX_ProjectRevision_Number",
                table: "ProjectRevision",
                columns: new[] { "ProjectId", "RevisionNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShareLink_CreatedByUserId",
                table: "ShareLink",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ShareLink_ProjectId",
                table: "ShareLink",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "UX_ShareLink_Token",
                table: "ShareLink",
                column: "Token",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AppUser_BimCoordinatorUserId",
                table: "Project",
                column: "BimCoordinatorUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AppUser_CreatedByUserId",
                table: "Project",
                column: "CreatedByUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AppUser_ProjectManagerUserId",
                table: "Project",
                column: "ProjectManagerUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_BimCoordinatorUserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_CreatedByUserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_ProjectManagerUserId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "ApprovalItem");

            migrationBuilder.DropTable(
                name: "AuditLogEntry");

            migrationBuilder.DropTable(
                name: "FinalSubmission");

            migrationBuilder.DropTable(
                name: "ItemResponse");

            migrationBuilder.DropTable(
                name: "ShareLink");

            migrationBuilder.DropTable(
                name: "ApprovalCategory");

            migrationBuilder.DropTable(
                name: "ItemIdentity");

            migrationBuilder.DropTable(
                name: "ProjectRevision");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropIndex(
                name: "IX_Project_BimCoordinatorUserId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_ProjectManagerUserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "BimCoordinatorUserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "ProjectManagerUserId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_CreatedByUserId",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Project");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Project",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Project_CreatedByUserId",
                table: "Project",
                column: "CreatedByUserId",
                filter: "[CreatedByUserId] IS NOT NULL");

            migrationBuilder.AddColumn<string>(
                name: "BimCoordinator",
                table: "Project",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedByName",
                table: "Project",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProjectManager",
                table: "Project",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);
        }
    }
}
