using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.WithdrawCompany.ReportingToolLimit;

public record Company5TabWithDrawReportingToolLimitCommand(int CompanyId, int UserId) : IRequest<bool>;

public class Company5TabWithDrawReportingToolLimitCommandHandler : IRequestHandler<Company5TabWithDrawReportingToolLimitCommand, bool>
{
    private readonly IReportingToolLimitsHistoryRepository _reportingToolLimitHistoryRepository;

    public Company5TabWithDrawReportingToolLimitCommandHandler(IReportingToolLimitsHistoryRepository reportingToolLimitHistoryRepository)
    {
        _reportingToolLimitHistoryRepository = reportingToolLimitHistoryRepository;
    }

    public async Task<bool> Handle(Company5TabWithDrawReportingToolLimitCommand request, CancellationToken cancellationToken)
    {
        return await _reportingToolLimitHistoryRepository.WithdrawCompany5TabReportingToolLimitsHistoryAsync(request.CompanyId, request.UserId);
    }
}
