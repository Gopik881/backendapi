using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class ReportingToolLimitsRepository : IReportingToolLimitsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public ReportingToolLimitsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> Company5TabApproveReportingToolLimitsDataAsync(int companyId, int userId, Company5TabReportingToolLimitsDto Company5TabReportingTool, CancellationToken cancellationToken = default)
    {

        _dbContext.ReportingToolLimits.Add(new ReportingToolLimit
        {
            CompanyId = companyId,
            NoOfReportingAdmins = Company5TabReportingTool.NoOfReportingAdmins,
            NoOfCustomReportCreators = Company5TabReportingTool.NoOfCustomReportCreators,
            SavedReportQueriesInLibrary = Company5TabReportingTool.NoOfSavedReportQueriesCompany,
            DashboardsInLibrary = Company5TabReportingTool.NoOfDashboardsCompany,
            SavedReportQueriesPerUser = Company5TabReportingTool.NoOfSavedReportQueriesUsers,
            DashboardsInPersonalLibrary = Company5TabReportingTool.NoOfDashboardsUsers,
            LetterGenerationAdmins = Company5TabReportingTool.NoOfLetterGenerationAdmin,
            TemplatesSaved = Company5TabReportingTool.NoOfTemplatesSaved,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow
        });

        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }


    
}
