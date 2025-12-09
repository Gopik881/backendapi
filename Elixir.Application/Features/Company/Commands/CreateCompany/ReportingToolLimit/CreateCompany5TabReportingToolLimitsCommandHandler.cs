using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateCompany5TabReportingToolLimitsCommand(int companyId, int userId, Company5TabReportingToolLimitsDto ReportingToolLimitsDto) : IRequest<bool>;

public class CreateCompany5TabReportingToolLimitsCommandHandler : IRequestHandler<CreateCompany5TabReportingToolLimitsCommand, bool>
{
    private readonly IReportingToolLimitsHistoryRepository _reportingToolLimitsHistoryRepository;

    public CreateCompany5TabReportingToolLimitsCommandHandler(IReportingToolLimitsHistoryRepository reportingToolLimitsHistoryRepository)
    {
        _reportingToolLimitsHistoryRepository = reportingToolLimitsHistoryRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabReportingToolLimitsCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _reportingToolLimitsHistoryRepository.Company5TabCreateReportingToolLimitsDataAsync(request.companyId, request.userId, request.ReportingToolLimitsDto);
    }
}
