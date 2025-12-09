using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.AccountHistory;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyAdmin;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.CompanyHistory;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ElixirUserHistory;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.EscalationContacts;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ModuleMappingHistory;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.ReportingToolHistory;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistoryByVersionNumber.Company5TabHistoryByVersionCompositeCommandHandler;

public record GetCompositeCompany5TabHistoryCommand(int userId, int CompanyId, int VersionNumber) : IRequest<object>;

public class GetHistoryByVersionCompositeCommandHandler : IRequestHandler<GetCompositeCompany5TabHistoryCommand, object>
{
    private readonly IMediator _mediator;

    public GetHistoryByVersionCompositeCommandHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<object> Handle(GetCompositeCompany5TabHistoryCommand request, CancellationToken cancellationToken)
    {
        var companyHistory = await _mediator.Send(new GetCompany5TabCompanyHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var accountHistory = await _mediator.Send(new GetCompany5TabAccountHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var companyAdminHistory = await _mediator.Send(new GetCompany5TabCompanyAdminHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var moduleMappingHistory = await _mediator.Send(new GetCompany5TabModuleMappingHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var reportingToolLimitHistory = await _mediator.Send(new GetCompany5TabReportingToolLimitHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var escalationContactsHistory = await _mediator.Send(new GetCompany5TabEscalationContactsHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);
        ////var elixirUserHistory = await _mediator.Send(new GetCompany5TabElixirUserHistoryQuery(request.userId, request.CompanyId, request.VersionNumber), cancellationToken);

      
        ////// Pseudocode plan:
        ////// 1. Use reflection to get all public properties of Company5TabHistoryDto that are of type object or a known DTO type.
        ////// 2. Create a mapping between property names and the corresponding history objects.
        ////// 3. Assign the values dynamically to the dictionary, using the property names as keys.

        ////var result = new Company5TabHistoryDto();
        ////var historyObjects = new Dictionary<string, object>
        ////{
        ////    { nameof(companyHistory), companyHistory },
        ////    { nameof(accountHistory), accountHistory },
        ////    { nameof(companyAdminHistory), companyAdminHistory },
        ////    { nameof(moduleMappingHistory), moduleMappingHistory },
        ////    { nameof(reportingToolLimitHistory), reportingToolLimitHistory },
        ////    { nameof(escalationContactsHistory), escalationContactsHistory },
        ////    { nameof(elixirUserHistory), elixirUserHistory }
        ////};

        //foreach (var kvp in historyObjects)
        //{
        //    // Adjust key naming to match the expected keys in Company5TabHistoryDto
        //    // Use PascalCase and append "Dto" to the base name (e.g., "CompanyHistoryDto")
        //    var key = kvp.Key switch
        //    {
        //        nameof(companyHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_COMPANYHISTORYBYVERSION,
        //        nameof(accountHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_ACCOUNTHISTORYBYVERSION,
        //        nameof(companyAdminHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_COMPANYADMINHISTORYBYVERSION,
        //        nameof(moduleMappingHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_MODULEMAPPINGHISTORYBYVERSION,
        //        nameof(reportingToolLimitHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_REPORTINGTOOLLIMITHISTORYBYVERSION,
        //        nameof(escalationContactsHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_ESCALATIONCONTACTHISTORYBYVERSION,
        //        nameof(elixirUserHistory) => AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_ELIXIRHRUSERHISTORYBYVERSION,
        //        _ => kvp.Key + AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_ELIXIRHRUSERHISTORYBYVERSION
        //    };
        //    result.Company5TabHistory[key] = kvp.Value;
        //}
        return companyHistory;
    }
}
