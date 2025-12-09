using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.ReportingToolLimit;

public record ApproveCompany5TabReportingToolLimitsCommand(int companyId, int userId, Company5TabReportingToolLimitsDto ReportingToolLimitsDto) : IRequest<bool>;

public class ApproveCompany5TabReportingToolLimitsCommandHandler : IRequestHandler<ApproveCompany5TabReportingToolLimitsCommand, bool>
{
    private readonly IReportingToolLimitsRepository _reportingToolLimitsRepository;

    public ApproveCompany5TabReportingToolLimitsCommandHandler(IReportingToolLimitsRepository reportingToolLimitsRepository)
    {
        _reportingToolLimitsRepository = reportingToolLimitsRepository;
    }

    public async Task<bool> Handle(ApproveCompany5TabReportingToolLimitsCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _reportingToolLimitsRepository.Company5TabApproveReportingToolLimitsDataAsync(request.companyId, request.userId, request.ReportingToolLimitsDto);
    }
}