using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record GetUserProfileByUserIdQuery(int UserId, bool IsSuperUser) : IRequest<UserProfileDto>;

public class GetUserProfileByUserIdQueryHandler : IRequestHandler<GetUserProfileByUserIdQuery, UserProfileDto>
{
    private readonly IUsersRepository _userRepository;

    public GetUserProfileByUserIdQueryHandler(IUsersRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileByUserIdQuery request, CancellationToken cancellationToken)
    {
        return await _userRepository.GetUserProfileByUserIdAsync(request.UserId, request.IsSuperUser);
    }
}
