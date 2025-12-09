using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportAccess
{
    public record DeleteReportAccessCommand(int UserGroupId) : IRequest<bool>;

    public class DeleteReportAccessCommandHandler : IRequestHandler<DeleteReportAccessCommand, bool>
    {
        private readonly IReportAccessRepository _reportAccessRepository;

        public DeleteReportAccessCommandHandler(IReportAccessRepository reportAccessRepository)
        {
            _reportAccessRepository = reportAccessRepository;
        }

        public async Task<bool> Handle(DeleteReportAccessCommand request, CancellationToken cancellationToken)
        {
            return await _reportAccessRepository.DeleteReportAccessByUserGroupIdAsync(request.UserGroupId);
        }
    }
}
