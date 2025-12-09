using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record AddReportAccessCommand(int GroupId, ReportingAccessDto ReportAccess) : IRequest<bool>;

public class AddReportAccessCommandHandler : IRequestHandler<AddReportAccessCommand, bool>
{
    private readonly IReportAccessRepository _reportAccessRepository;

    public AddReportAccessCommandHandler(IReportAccessRepository reportAccessRepository)
    {
        _reportAccessRepository = reportAccessRepository;
    }

    public async Task<bool> Handle(AddReportAccessCommand request, CancellationToken cancellationToken)
    {
        return await _reportAccessRepository.AddReportAccessAsync(request.GroupId, request.ReportAccess);
    }
}