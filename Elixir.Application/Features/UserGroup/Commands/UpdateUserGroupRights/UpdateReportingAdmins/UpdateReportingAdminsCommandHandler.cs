using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateReportingAdmins;

public record UpdateReportingAdminsCommand(int UserGroupId, List<UserGroupReportingAdmin> ReportingAdminsDtos) : IRequest<bool>;

public class UpdateReportingAdminsCommandHandler : IRequestHandler<UpdateReportingAdminsCommand, bool>
{
    private readonly IReportingAdminRepository _reportingAdminsRepository;

    public UpdateReportingAdminsCommandHandler(IReportingAdminRepository reportingAdminsRepository)
    {
        _reportingAdminsRepository = reportingAdminsRepository;
    }

    public async Task<bool> Handle(UpdateReportingAdminsCommand request, CancellationToken cancellationToken)
    {
        // Save reporting admins data for the user group
        return await _reportingAdminsRepository.UpdateReportingAdminsAsync(request.UserGroupId,request.ReportingAdminsDtos);
    }
}
