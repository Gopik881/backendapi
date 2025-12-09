using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Queries.GetMyProfile;

public record GetUserProfileQuery(string EmailId,bool IsSuperAdmin) : IRequest<UserProfileDto>;

public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, UserProfileDto>
{
    private readonly IUsersRepository _userRepository;
    private readonly ISuperUsersRepository _superUsersRepository;
    private readonly ICryptoService _cryptoService;

    public GetUserProfileQueryHandler(IUsersRepository userRepository, ICryptoService cryptoService, ISuperUsersRepository superUsersRepository)
    {
        _userRepository = userRepository;
        _cryptoService = cryptoService;
        _superUsersRepository = superUsersRepository;
    }

    public async Task<UserProfileDto> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        if(request.IsSuperAdmin)
        {
            return await _superUsersRepository.GetUserProfileAsync(request.EmailId, _cryptoService.GenerateIntegerHashForString(request.EmailId));
        }
        return await _userRepository.GetUserProfileAsync(request.EmailId,_cryptoService.GenerateIntegerHashForString(request.EmailId));
    }
}
