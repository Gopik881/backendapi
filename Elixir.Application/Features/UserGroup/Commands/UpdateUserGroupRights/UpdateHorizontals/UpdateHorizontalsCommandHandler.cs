using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Commands.UpdateUserGroupRights.UpdateHorizontals;

public record UpdateHorizontalsCommand(int UserGroupId, List<UserGroupHorizontals> HorizontalDto) : IRequest<bool>;

public class UpdateHorizontalsCommandHandler : IRequestHandler<UpdateHorizontalsCommand, bool>
{
    private readonly IHorizontalsRepository _horizontalsRepository;

    public UpdateHorizontalsCommandHandler(IHorizontalsRepository horizontalsRepository)
    {
        _horizontalsRepository = horizontalsRepository;
    }

    public async Task<bool> Handle(UpdateHorizontalsCommand request, CancellationToken cancellationToken)
    {
        // Save horizontal rights data for the user group
        return await _horizontalsRepository.UpdateHorizontalsAsync(request.UserGroupId,request.HorizontalDto);
    }
}
