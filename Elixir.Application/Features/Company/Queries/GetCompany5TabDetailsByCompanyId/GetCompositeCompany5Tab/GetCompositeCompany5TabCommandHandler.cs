using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetAccount;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompany;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompanyAdmin;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetElixirUser;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetEscalationContacts;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetModuleMapping;
using Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetReportingToolLimit;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabDetailsByCompanyId.GetCompositeCompany5Tab;

public record GetCompositeCompany5TabCommand(int CompanyId, int userId, bool IsPrevious) : IRequest<Company5TabDto>;

public class GetCompositeCompany5TabCommandHandler : IRequestHandler<GetCompositeCompany5TabCommand, Company5TabDto>
{
    private readonly IMediator _mediator;
    private readonly ICompanyOnboardingStatusRepository _companyOnboardingStatusRepository;
    public GetCompositeCompany5TabCommandHandler(IMediator mediator, ICompanyOnboardingStatusRepository companyOnboardingStatusRepository)
    {
        _mediator = mediator;
        _companyOnboardingStatusRepository = companyOnboardingStatusRepository;
    }

    public async Task<Company5TabDto> Handle(GetCompositeCompany5TabCommand request, CancellationToken cancellationToken)
    {
        //// Get the current onboarding status for the company
        var currentOnboardingStatus = await _companyOnboardingStatusRepository.GetCompanyOnBoardingStatus(request.CompanyId);

        if(currentOnboardingStatus == AppConstants.ONBOARDING_STATUS_NEW)
        {
            return new Company5TabDto();
        }
        //// Use a local variable for IsPrevious, since request.IsPrevious is init-only
        var isPrevious = request.IsPrevious;

        //if (currentOnboardingStatus == AppConstants.ONBOARDING_STATUS_REJECTED)
        //{
        //    isPrevious = true;  
        //}

        // Get Company Data
        var companyData = await _mediator.Send(new GetCompany5TabCompanyDataQuery(request.CompanyId, request.userId, isPrevious), cancellationToken);

        // Get Account Data
        var accountData = await _mediator.Send(new GetAccountCompany5TabAccountQuery(request.CompanyId, isPrevious), cancellationToken);

        // Get Company Admin Data
        var companyAdminData = await _mediator.Send(new GetCompany5TabCompanyAdminQuery(request.CompanyId, isPrevious), cancellationToken);

        // Get Module Mapping Data
        var moduleMappingData = await _mediator.Send(new GetCompany5TabModuleMappingQuery(request.CompanyId, isPrevious), cancellationToken);

        // Get Reporting Tool Limits Data
        var reportingToolLimitsData = await _mediator.Send(new GetCompany5TabReportingToolLimitQuery(request.CompanyId, isPrevious), cancellationToken);

        // Get Escalation Contacts Data
        var escalationContactsData = await _mediator.Send(new GetCompany5TabEscalationContactsQuery(request.CompanyId, isPrevious), cancellationToken);

        // Get Elixir User Data
        var elixirUserData = await _mediator.Send(new GetCompany5TabElixirUserQuery(request.CompanyId, isPrevious), cancellationToken);

        // Combine all into Company5TabDto
        var result = new Company5TabDto
        {
            Company5TabCompanyDto = companyData,
            Company5TabAccountDto = accountData,
            Company5TabCompanyAdminDto = companyAdminData,
            Company5TabModuleMappingDto = moduleMappingData,
            Company5TabReportingToolLimitsDto = reportingToolLimitsData,
            Company5TabEscalationContactDto = escalationContactsData,
            company5TabElixirUserDto = elixirUserData
        };

        return result;
    }
}
