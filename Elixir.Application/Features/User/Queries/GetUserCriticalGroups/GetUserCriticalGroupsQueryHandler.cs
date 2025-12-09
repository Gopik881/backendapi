using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.User.Queries.GetUserCriticalGroups;

public record GetUserCriticalGroupsQuery(int UserId) : IRequest<List<string>>;

public class GetUserCriticalGroupsQueryHandler : IRequestHandler<GetUserCriticalGroupsQuery, List<string>>
{
    private readonly IUsersRepository _userRepository;
    public GetUserCriticalGroupsQueryHandler(IUsersRepository userRepository)
    {
        _userRepository = userRepository;
    }
    public async Task<List<string>> Handle(GetUserCriticalGroupsQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUsersCriticalGroupAsync(request.UserId);
    }
}
