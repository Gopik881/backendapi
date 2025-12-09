using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Commands.UpdateUser;

public record UpdateUserCommand(int userId, UserProfileDto UpdateUserDto) : IRequest<bool>;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly IUsersRepository _userRepository;
    private readonly ICryptoService _cryptoService;
    private readonly INotificationsRepository _notificationsRepository;
    public UpdateUserCommandHandler(IUsersRepository userRepository, ICryptoService cryptoService, INotificationsRepository notificationsRepository)
    {
        _userRepository = userRepository;
        _cryptoService = cryptoService;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        
        var existsUser = await _userRepository.GetUserProfileByUserIdAsync(request.UpdateUserDto.UserId);
        // Check if email already exists for another user (exclude current user)
        var emailHash = _cryptoService.GenerateIntegerHashForString(request.UpdateUserDto.EmailId);

        var existingemailHash = _cryptoService.GenerateIntegerHashForString(existsUser.EmailId);
        //var existingemailHash = _cryptoService.GenerateIntegerHashForString(request.UpdateUserDto.EmailId);
        // If another user (different userId) has this email, block update
        if (existsUser.EmailId != request.UpdateUserDto.EmailId)
        {
            await _userRepository.UpdateUserEmailHasPasswordAsync(existsUser.EmailId, existingemailHash, emailHash, null, null);
            //await _userRepository.UpdateUserPasswordAsync(existsUser.EmailId, emailHash, null, null);            
        }

        await _userRepository.UpdateUserAsync(request.UpdateUserDto, emailHash);
        return await _userRepository.SetResetPasswordTokenEmptyAsync(request.UpdateUserDto.EmailId, null);
    }
}
