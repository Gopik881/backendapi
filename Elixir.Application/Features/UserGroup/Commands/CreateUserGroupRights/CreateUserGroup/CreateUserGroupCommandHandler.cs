using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record CreateUserGroupCommand(CreateUserGroupDto CreateUserGroupDto) : IRequest<int>;

public class CreateUserGroupCommandHandler : IRequestHandler<CreateUserGroupCommand, int>
{
    private readonly IUserGroupsRepository _userGroupRepository;

    public CreateUserGroupCommandHandler(IUserGroupsRepository userGroupRepository)
    {
        _userGroupRepository = userGroupRepository;
    }

    public async Task<int> Handle(CreateUserGroupCommand request, CancellationToken cancellationToken)
    {
        return await _userGroupRepository.CreateUserGroupAsync(request.CreateUserGroupDto);
    }
}