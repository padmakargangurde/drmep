using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTablesToMepHubPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalCategory_ProjectRevision_ProjectRevisionId",
                table: "ApprovalCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalItem_ApprovalCategory_ApprovalCategoryId",
                table: "ApprovalItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalItem_ApprovalItem_PreviousItemId",
                table: "ApprovalItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalItem_ItemIdentity_ItemIdentityId",
                table: "ApprovalItem");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalItem_ProjectRevision_ProjectRevisionId",
                table: "ApprovalItem");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogEntry_AppUser_ActorUserId",
                table: "AuditLogEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogEntry_Project_ProjectId",
                table: "AuditLogEntry");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalSubmission_AppUser_ReopenedByUserId",
                table: "FinalSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalSubmission_ProjectRevision_ProjectRevisionId",
                table: "FinalSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_FinalSubmission_Project_ProjectId",
                table: "FinalSubmission");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemIdentity_Project_ProjectId",
                table: "ItemIdentity");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemResponse_ItemIdentity_ItemIdentityId",
                table: "ItemResponse");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_BimCoordinatorUserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_CreatedByUserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_AppUser_ProjectManagerUserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRevision_AppUser_UploadedByUserId",
                table: "ProjectRevision");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectRevision_Project_ProjectId",
                table: "ProjectRevision");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareLink_AppUser_CreatedByUserId",
                table: "ShareLink");

            migrationBuilder.DropForeignKey(
                name: "FK_ShareLink_Project_ProjectId",
                table: "ShareLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ShareLink",
                table: "ShareLink");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProjectRevision",
                table: "ProjectRevision");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Project",
                table: "Project");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemResponse",
                table: "ItemResponse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ItemIdentity",
                table: "ItemIdentity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FinalSubmission",
                table: "FinalSubmission");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogEntry",
                table: "AuditLogEntry");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppUser",
                table: "AppUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalItem",
                table: "ApprovalItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApprovalCategory",
                table: "ApprovalCategory");

            migrationBuilder.RenameTable(
                name: "ShareLink",
                newName: "Mep_hub_ShareLinks");

            migrationBuilder.RenameTable(
                name: "ProjectRevision",
                newName: "Mep_hub_ProjectRevisions");

            migrationBuilder.RenameTable(
                name: "Project",
                newName: "Mep_hub_Projects");

            migrationBuilder.RenameTable(
                name: "ItemResponse",
                newName: "Mep_hub_ItemResponses");

            migrationBuilder.RenameTable(
                name: "ItemIdentity",
                newName: "Mep_hub_ItemIdentities");

            migrationBuilder.RenameTable(
                name: "FinalSubmission",
                newName: "Mep_hub_FinalSubmissions");

            migrationBuilder.RenameTable(
                name: "AuditLogEntry",
                newName: "Mep_hub_AuditLogEntries");

            migrationBuilder.RenameTable(
                name: "AppUser",
                newName: "Mep_hub_Users");

            migrationBuilder.RenameTable(
                name: "ApprovalItem",
                newName: "Mep_hub_ApprovalItems");

            migrationBuilder.RenameTable(
                name: "ApprovalCategory",
                newName: "Mep_hub_ApprovalCategories");

            migrationBuilder.RenameIndex(
                name: "IX_ShareLink_ProjectId",
                table: "Mep_hub_ShareLinks",
                newName: "IX_Mep_hub_ShareLinks_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_ShareLink_CreatedByUserId",
                table: "Mep_hub_ShareLinks",
                newName: "IX_Mep_hub_ShareLinks_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProjectRevision_UploadedByUserId",
                table: "Mep_hub_ProjectRevisions",
                newName: "IX_Mep_hub_ProjectRevisions_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_ProjectManagerUserId",
                table: "Mep_hub_Projects",
                newName: "IX_Mep_hub_Projects_ProjectManagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_BimCoordinatorUserId",
                table: "Mep_hub_Projects",
                newName: "IX_Mep_hub_Projects_BimCoordinatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FinalSubmission_ReopenedByUserId",
                table: "Mep_hub_FinalSubmissions",
                newName: "IX_Mep_hub_FinalSubmissions_ReopenedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_FinalSubmission_ProjectRevisionId",
                table: "Mep_hub_FinalSubmissions",
                newName: "IX_Mep_hub_FinalSubmissions_ProjectRevisionId");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogEntry_ActorUserId",
                table: "Mep_hub_AuditLogEntries",
                newName: "IX_Mep_hub_AuditLogEntries_ActorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalItem_PreviousItemId",
                table: "Mep_hub_ApprovalItems",
                newName: "IX_Mep_hub_ApprovalItems_PreviousItemId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalItem_ItemIdentityId",
                table: "Mep_hub_ApprovalItems",
                newName: "IX_Mep_hub_ApprovalItems_ItemIdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_ApprovalItem_ApprovalCategoryId",
                table: "Mep_hub_ApprovalItems",
                newName: "IX_Mep_hub_ApprovalItems_ApprovalCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ShareLinks",
                table: "Mep_hub_ShareLinks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ProjectRevisions",
                table: "Mep_hub_ProjectRevisions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_Projects",
                table: "Mep_hub_Projects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ItemResponses",
                table: "Mep_hub_ItemResponses",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ItemIdentities",
                table: "Mep_hub_ItemIdentities",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_FinalSubmissions",
                table: "Mep_hub_FinalSubmissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_AuditLogEntries",
                table: "Mep_hub_AuditLogEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_Users",
                table: "Mep_hub_Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ApprovalItems",
                table: "Mep_hub_ApprovalItems",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Mep_hub_ApprovalCategories",
                table: "Mep_hub_ApprovalCategories",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ApprovalCategories_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_ApprovalCategories",
                column: "ProjectRevisionId",
                principalTable: "Mep_hub_ProjectRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ApprovalCategories_ApprovalCategoryId",
                table: "Mep_hub_ApprovalItems",
                column: "ApprovalCategoryId",
                principalTable: "Mep_hub_ApprovalCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ApprovalItems_PreviousItemId",
                table: "Mep_hub_ApprovalItems",
                column: "PreviousItemId",
                principalTable: "Mep_hub_ApprovalItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ItemIdentities_ItemIdentityId",
                table: "Mep_hub_ApprovalItems",
                column: "ItemIdentityId",
                principalTable: "Mep_hub_ItemIdentities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_ApprovalItems",
                column: "ProjectRevisionId",
                principalTable: "Mep_hub_ProjectRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_AuditLogEntries_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_AuditLogEntries",
                column: "ProjectId",
                principalTable: "Mep_hub_Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_AuditLogEntries_Mep_hub_Users_ActorUserId",
                table: "Mep_hub_AuditLogEntries",
                column: "ActorUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_FinalSubmissions",
                column: "ProjectRevisionId",
                principalTable: "Mep_hub_ProjectRevisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_FinalSubmissions",
                column: "ProjectId",
                principalTable: "Mep_hub_Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_Users_ReopenedByUserId",
                table: "Mep_hub_FinalSubmissions",
                column: "ReopenedByUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ItemIdentities_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ItemIdentities",
                column: "ProjectId",
                principalTable: "Mep_hub_Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ItemResponses_Mep_hub_ItemIdentities_ItemIdentityId",
                table: "Mep_hub_ItemResponses",
                column: "ItemIdentityId",
                principalTable: "Mep_hub_ItemIdentities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ProjectRevisions_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ProjectRevisions",
                column: "ProjectId",
                principalTable: "Mep_hub_Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ProjectRevisions_Mep_hub_Users_UploadedByUserId",
                table: "Mep_hub_ProjectRevisions",
                column: "UploadedByUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_BimCoordinatorUserId",
                table: "Mep_hub_Projects",
                column: "BimCoordinatorUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_CreatedByUserId",
                table: "Mep_hub_Projects",
                column: "CreatedByUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_ProjectManagerUserId",
                table: "Mep_hub_Projects",
                column: "ProjectManagerUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ShareLinks_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ShareLinks",
                column: "ProjectId",
                principalTable: "Mep_hub_Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Mep_hub_ShareLinks_Mep_hub_Users_CreatedByUserId",
                table: "Mep_hub_ShareLinks",
                column: "CreatedByUserId",
                principalTable: "Mep_hub_Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ApprovalCategories_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_ApprovalCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ApprovalCategories_ApprovalCategoryId",
                table: "Mep_hub_ApprovalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ApprovalItems_PreviousItemId",
                table: "Mep_hub_ApprovalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ItemIdentities_ItemIdentityId",
                table: "Mep_hub_ApprovalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ApprovalItems_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_ApprovalItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_AuditLogEntries_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_AuditLogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_AuditLogEntries_Mep_hub_Users_ActorUserId",
                table: "Mep_hub_AuditLogEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_ProjectRevisions_ProjectRevisionId",
                table: "Mep_hub_FinalSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_FinalSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_FinalSubmissions_Mep_hub_Users_ReopenedByUserId",
                table: "Mep_hub_FinalSubmissions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ItemIdentities_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ItemIdentities");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ItemResponses_Mep_hub_ItemIdentities_ItemIdentityId",
                table: "Mep_hub_ItemResponses");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ProjectRevisions_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ProjectRevisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ProjectRevisions_Mep_hub_Users_UploadedByUserId",
                table: "Mep_hub_ProjectRevisions");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_BimCoordinatorUserId",
                table: "Mep_hub_Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_CreatedByUserId",
                table: "Mep_hub_Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_Projects_Mep_hub_Users_ProjectManagerUserId",
                table: "Mep_hub_Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ShareLinks_Mep_hub_Projects_ProjectId",
                table: "Mep_hub_ShareLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_Mep_hub_ShareLinks_Mep_hub_Users_CreatedByUserId",
                table: "Mep_hub_ShareLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_Users",
                table: "Mep_hub_Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ShareLinks",
                table: "Mep_hub_ShareLinks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_Projects",
                table: "Mep_hub_Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ProjectRevisions",
                table: "Mep_hub_ProjectRevisions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ItemResponses",
                table: "Mep_hub_ItemResponses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ItemIdentities",
                table: "Mep_hub_ItemIdentities");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_FinalSubmissions",
                table: "Mep_hub_FinalSubmissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_AuditLogEntries",
                table: "Mep_hub_AuditLogEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ApprovalItems",
                table: "Mep_hub_ApprovalItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Mep_hub_ApprovalCategories",
                table: "Mep_hub_ApprovalCategories");

            migrationBuilder.RenameTable(
                name: "Mep_hub_Users",
                newName: "AppUser");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ShareLinks",
                newName: "ShareLink");

            migrationBuilder.RenameTable(
                name: "Mep_hub_Projects",
                newName: "Project");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ProjectRevisions",
                newName: "ProjectRevision");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ItemResponses",
                newName: "ItemResponse");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ItemIdentities",
                newName: "ItemIdentity");

            migrationBuilder.RenameTable(
                name: "Mep_hub_FinalSubmissions",
                newName: "FinalSubmission");

            migrationBuilder.RenameTable(
                name: "Mep_hub_AuditLogEntries",
                newName: "AuditLogEntry");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ApprovalItems",
                newName: "ApprovalItem");

            migrationBuilder.RenameTable(
                name: "Mep_hub_ApprovalCategories",
                newName: "ApprovalCategory");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ShareLinks_ProjectId",
                table: "ShareLink",
                newName: "IX_ShareLink_ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ShareLinks_CreatedByUserId",
                table: "ShareLink",
                newName: "IX_ShareLink_CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_Projects_ProjectManagerUserId",
                table: "Project",
                newName: "IX_Project_ProjectManagerUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_Projects_BimCoordinatorUserId",
                table: "Project",
                newName: "IX_Project_BimCoordinatorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ProjectRevisions_UploadedByUserId",
                table: "ProjectRevision",
                newName: "IX_ProjectRevision_UploadedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_FinalSubmissions_ReopenedByUserId",
                table: "FinalSubmission",
                newName: "IX_FinalSubmission_ReopenedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_FinalSubmissions_ProjectRevisionId",
                table: "FinalSubmission",
                newName: "IX_FinalSubmission_ProjectRevisionId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_AuditLogEntries_ActorUserId",
                table: "AuditLogEntry",
                newName: "IX_AuditLogEntry_ActorUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ApprovalItems_PreviousItemId",
                table: "ApprovalItem",
                newName: "IX_ApprovalItem_PreviousItemId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ApprovalItems_ItemIdentityId",
                table: "ApprovalItem",
                newName: "IX_ApprovalItem_ItemIdentityId");

            migrationBuilder.RenameIndex(
                name: "IX_Mep_hub_ApprovalItems_ApprovalCategoryId",
                table: "ApprovalItem",
                newName: "IX_ApprovalItem_ApprovalCategoryId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppUser",
                table: "AppUser",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ShareLink",
                table: "ShareLink",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Project",
                table: "Project",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProjectRevision",
                table: "ProjectRevision",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemResponse",
                table: "ItemResponse",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ItemIdentity",
                table: "ItemIdentity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FinalSubmission",
                table: "FinalSubmission",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogEntry",
                table: "AuditLogEntry",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalItem",
                table: "ApprovalItem",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApprovalCategory",
                table: "ApprovalCategory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalCategory_ProjectRevision_ProjectRevisionId",
                table: "ApprovalCategory",
                column: "ProjectRevisionId",
                principalTable: "ProjectRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalItem_ApprovalCategory_ApprovalCategoryId",
                table: "ApprovalItem",
                column: "ApprovalCategoryId",
                principalTable: "ApprovalCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalItem_ApprovalItem_PreviousItemId",
                table: "ApprovalItem",
                column: "PreviousItemId",
                principalTable: "ApprovalItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalItem_ItemIdentity_ItemIdentityId",
                table: "ApprovalItem",
                column: "ItemIdentityId",
                principalTable: "ItemIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalItem_ProjectRevision_ProjectRevisionId",
                table: "ApprovalItem",
                column: "ProjectRevisionId",
                principalTable: "ProjectRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogEntry_AppUser_ActorUserId",
                table: "AuditLogEntry",
                column: "ActorUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogEntry_Project_ProjectId",
                table: "AuditLogEntry",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalSubmission_AppUser_ReopenedByUserId",
                table: "FinalSubmission",
                column: "ReopenedByUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalSubmission_ProjectRevision_ProjectRevisionId",
                table: "FinalSubmission",
                column: "ProjectRevisionId",
                principalTable: "ProjectRevision",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FinalSubmission_Project_ProjectId",
                table: "FinalSubmission",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemIdentity_Project_ProjectId",
                table: "ItemIdentity",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemResponse_ItemIdentity_ItemIdentityId",
                table: "ItemResponse",
                column: "ItemIdentityId",
                principalTable: "ItemIdentity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRevision_AppUser_UploadedByUserId",
                table: "ProjectRevision",
                column: "UploadedByUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectRevision_Project_ProjectId",
                table: "ProjectRevision",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShareLink_AppUser_CreatedByUserId",
                table: "ShareLink",
                column: "CreatedByUserId",
                principalTable: "AppUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ShareLink_Project_ProjectId",
                table: "ShareLink",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
