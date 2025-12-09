using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportingAdmins
{
    public record DeleteReportingAdminsCommand(int UserGroupId) : IRequest<bool>;

    public class DeleteReportingAdminsCommandHandler : IRequestHandler<DeleteReportingAdminsCommand, bool>
    {
        private readonly IReportingAdminRepository _reportingAdminRepository;

        public DeleteReportingAdminsCommandHandler(IReportingAdminRepository reportingAdminRepository)
        {
            _reportingAdminRepository = reportingAdminRepository;
        }

        public async Task<bool> Handle(DeleteReportingAdminsCommand request, CancellationToken cancellationToken)
        {
            return await _reportingAdminRepository.DeleteReportingAdminsByUserGroupIdAsync(request.UserGroupId);
        }
    }
}
