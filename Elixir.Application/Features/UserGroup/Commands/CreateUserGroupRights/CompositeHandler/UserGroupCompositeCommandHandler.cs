using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.CreateUserGroupRights.CompositeHandler
{
    public record UserGroupCompositeCommand(CreateUserGroupDto CreateUserGroupDto) : IRequest<object>;

    public class UserGroupCompositeCommandHandler : IRequestHandler<UserGroupCompositeCommand, object>
    {
        private readonly Func<ITransactionRepository> _transactionRepositoryFactory;
        private readonly IMediator _mediator;
        private readonly IUserGroupsRepository _userGroupsRepository;

        private readonly IHorizontalsRepository _horizontalsRepository;
        private readonly IReportAccessRepository _reportingAccessRepository;
        private readonly IReportingAdminRepository _reportingAdminRepository;
        public UserGroupCompositeCommandHandler(Func<ITransactionRepository> transactionRepositoryFactory, IMediator mediator, IUserGroupsRepository userGroupsRepository, IHorizontalsRepository horizontalsRepository, IReportAccessRepository reportAccessRepository, IReportingAdminRepository reportingAdminRepository)
        {
            _transactionRepositoryFactory = transactionRepositoryFactory;
            _mediator = mediator;
            _userGroupsRepository = userGroupsRepository;
            _horizontalsRepository = horizontalsRepository;
            _reportingAccessRepository = reportAccessRepository;
            _reportingAdminRepository = reportingAdminRepository;
        }

        public async Task<object> Handle(UserGroupCompositeCommand request, CancellationToken cancellationToken)
        {
            var dto = request.CreateUserGroupDto;

            // Example: Check for required fields
            if (string.IsNullOrWhiteSpace(dto.UserGroupName))
                //errorMessages.Add("User group name is required.");
                throw new Exception("User group name is required.");

            // Fix for CS4034: Use an async lambda expression
            if (!await _userGroupsRepository.IsGroupNameAvailableAsync(dto.UserGroupName))
                //errorMessages.Add($"User group name '{dto.UserGroupName}' is already in use.");
                throw new Exception($"User group name '{dto.UserGroupName}' is already in use.");
           
            var duplicateGroups = await _userGroupsRepository.CheckForDuplicateGroupConfigurationsAsync(
                dto.UserGroupMenuRights ?? new List<UserGroupMenuRights>(),
                dto.userGroupHorizontals ?? new List<UserGroupHorizontals>(),
                dto.reportingAccessDto ?? new ReportingAccessDto(),
                dto.userGroupReportingAdmins ?? new List<UserGroupReportingAdmin>());

            if (duplicateGroups != null && duplicateGroups.Count > 0)
            {
                var duplicateGroupNames = string.Join(", ", duplicateGroups.Select(g => g.GroupName));
                throw new Exception($"Duplicate user rights found for the provided combination for userGroup(s): {duplicateGroupNames}.");
            }


            using var transactionRepository = _transactionRepositoryFactory();
            await transactionRepository.BeginTransactionAsync();
            try
            {
                var createUserGroupCommand = new CreateUserGroupCommand(dto);
                var result = await _mediator.Send(createUserGroupCommand, cancellationToken);
                int userGroupId = result;

                var addHorizontalsCommand = new AddHorizontalsCommand(userGroupId, dto.userGroupHorizontals ?? new List<UserGroupHorizontals>());
                if (!await _mediator.Send(addHorizontalsCommand, cancellationToken))
                    throw new Exception("Failed to add Horizontals.");

                if (dto.reportingAccessDto.Reports.Count > 0)
                {
                    var addReportAccessCommand = new AddReportAccessCommand(userGroupId, dto.reportingAccessDto ?? new ReportingAccessDto());
                    if (!await _mediator.Send(addReportAccessCommand, cancellationToken))
                        throw new Exception("Failed to add Report Access.");
                }

                if (dto.userGroupReportingAdmins != null && dto.userGroupReportingAdmins.Count > 0)
                {
                    var addReportingAdminsCommand = new AddReportingAdminsCommand(userGroupId, dto.userGroupReportingAdmins);
                    if (!await _mediator.Send(addReportingAdminsCommand, cancellationToken))
                        throw new Exception("Failed to add Reporting Admins.");
                }

                if (dto.UserGroupMenuRights != null || dto.UserGroupMenuRights.Count > 0)
                {
                    var addUserRightsCommand = new AddUserRightsCommand(userGroupId, dto.UserGroupMenuRights ?? new List<UserGroupMenuRights>());
                    if (!await _mediator.Send(addUserRightsCommand, cancellationToken))
                        throw new Exception("Failed to add User Rights.");
                }


                await transactionRepository.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transactionRepository.RollbackAsync();
                //return false;
                throw new Exception(AppConstants.ErrorCodes.USER_GROUP_CREATION_FAILED);
            }
        }
    }
}
