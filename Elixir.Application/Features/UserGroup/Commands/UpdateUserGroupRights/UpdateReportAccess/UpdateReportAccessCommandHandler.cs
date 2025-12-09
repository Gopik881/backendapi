using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportAccess;

public record UpdateReportAccessCommand(int UserGroupId, ReportingAccessDto ReportAccessDtos) : IRequest<bool>;

public class UpdateReportAccessCommandHandler : IRequestHandler<UpdateReportAccessCommand, bool>
{
    private readonly IReportAccessRepository _reportAccessRepository;

    public UpdateReportAccessCommandHandler(IReportAccessRepository reportAccessRepository)
    {
        _reportAccessRepository = reportAccessRepository;
    }

    public async Task<bool> Handle(UpdateReportAccessCommand request, CancellationToken cancellationToken)
    {
        // Save report access rights data for the user group
        return await _reportAccessRepository.UpdateReportAccessAsync(request.UserGroupId,request.ReportAccessDtos);
    }
}
