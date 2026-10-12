using System;
using Application.Contracts.Parsing;
using Application.Contracts.Repositories;
using Application.Contracts.UnitOfWork;
using Application.Features.AuditLog.Services;
using Application.Features.ClientReview.Services;
using Application.Features.FinalSubmission.Services;
using Application.Features.Projects.Services;
using Application.Features.Revisions.Services;
using Application.Features.ShareLinks.Services;
using Application.Features.Users.Services;
using Application.Services;
using Application.Contracts.Comments;
using Application.Contracts.ExcelFields;
using Application.Contracts.Exporting;
using Infrastructure.ExternalServices;
using Infrastructure.Parsing;
using Infrastructure.Persistence.DbContexts;
using Infrastructure.Reporting;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DIContainers
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' was not found.");

            // ---- Database --------------------------------------------------
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Repositories
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRevisionRepository, RevisionRepository>();
            services.AddScoped<IShareLinkRepository, ShareLinkRepository>();
            services.AddScoped<IClientReviewRepository, ClientReviewRepository>();
            services.AddScoped<IAuditLogRepository, AuditLogRepository>();
            services.AddScoped<IFinalSubmissionRepository, FinalSubmissionRepository>();


            //Services

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRevisionService, RevisionService>();
            services.AddScoped<IShareLinkService, ShareLinkService>();
            services.AddScoped<IFinalSubmissionService, FinalSubmissionService>();
            services.AddScoped<IClientReviewService, ClientReviewService>();
            services.AddScoped<IAuditLogService, AuditLogService>();

            // Parsing
            services.AddScoped<IWorkbookParser, WorkbookParser>();

            // Reporting
            services.AddScoped<IWorkbookExporter, WorkbookExporter>();

            // External services — MEP-APIs-V2 (MongoDB) holds Excel-sheet Fields data and comment threads
            services.AddHttpClient<IExcelFieldsClient, ExcelFieldsClient>((sp, client) =>
            {
                var baseUrl = configuration["MongoApi:BaseUrl"] ?? "http://localhost:5152/";
                client.BaseAddress = new Uri(baseUrl);
            });

            services.AddHttpClient<ICommentsClient, CommentsClient>((sp, client) =>
            {
                var baseUrl = configuration["MongoApi:BaseUrl"] ?? "http://localhost:5152/";
                client.BaseAddress = new Uri(baseUrl);
            });


            // Unit Of Work
            services.AddScoped<IUOW, UnitOfWork>();


            return services;
        }



      
    }
}