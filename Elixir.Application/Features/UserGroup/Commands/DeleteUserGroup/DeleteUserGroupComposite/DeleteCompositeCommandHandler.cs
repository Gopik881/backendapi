using Elixir.Application.Common.Constants;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteHorizontals;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportAccess;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteReportingAdmins;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupDetails;
using Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserRights;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.DeleteUserGroup.DeleteUserGroupComposite;

public record DeleteCompositeCommand(int UserGroupId) : IRequest<bool>;

public class DeleteCompositeCommandHandler : IRequestHandler<DeleteCompositeCommand, bool>
{
    private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
    private readonly IMediator _mediator;

    public DeleteCompositeCommandHandler(
        Func<ITransactionRepository> transactionRepositoryFactory,
        IMediator mediator)
    {
        _transactionRepositoryFactory = transactionRepositoryFactory;
        _mediator = mediator;
    }

    public async Task<bool> Handle(DeleteCompositeCommand request, CancellationToken cancellationToken)
    {
        using var transactionRepository = _transactionRepositoryFactory();
        await transactionRepository.BeginTransactionAsync();
        try
        {
            // Delete horizontals
            var horizontalsCommand = new DeleteHorizontalsCommand(request.UserGroupId);
            if (!await _mediator.Send(horizontalsCommand, cancellationToken))
                throw new Exception("Failed to delete horizontals.");

            // Delete report access
            var reportAccessCommand = new DeleteReportAccessCommand(request.UserGroupId);
            if (!await _mediator.Send(reportAccessCommand, cancellationToken))
                throw new Exception("Failed to delete report access.");

            // Delete reporting tool admins
            var reportingToolAdminsCommand = new DeleteReportingAdminsCommand(request.UserGroupId);
            if (!await _mediator.Send(reportingToolAdminsCommand, cancellationToken))
                throw new Exception("Failed to delete reporting tool admins.");

            // Delete user group details
            var userGroupDetailsCommand = new DeleteUserGroupDetailsCommand(request.UserGroupId);
            if (!await _mediator.Send(userGroupDetailsCommand, cancellationToken))
                throw new Exception("Failed to delete user group details.");

            // Delete user rights
            var userRightsCommand = new DeleteUserRightsCommand(request.UserGroupId);
            if (!await _mediator.Send(userRightsCommand, cancellationToken))
                throw new Exception("Failed to delete user rights.");

            await transactionRepository.CommitAsync();
            return true;
        }
        catch (Exception)
        {
            await transactionRepository.RollbackAsync();
            //return false;
            throw new Exception(AppConstants.ErrorCodes.USER_GROUP_DELETE_FAILED);
        }
    }
}
