using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record AddHorizontalsCommand(int GroupId, List<UserGroupHorizontals> Horizontals) : IRequest<bool>;

public class AddHorizontalsCommandHandler : IRequestHandler<AddHorizontalsCommand, bool>
{
    private readonly IHorizontalsRepository _horizontalsRepository;

    public AddHorizontalsCommandHandler(IHorizontalsRepository horizontalsRepository)
    {
        _horizontalsRepository = horizontalsRepository;
    }

    public async Task<bool> Handle(AddHorizontalsCommand request, CancellationToken cancellationToken)
    {
        return await _horizontalsRepository.AddHorizontalsAsync(request.GroupId, request.Horizontals);
    }
}