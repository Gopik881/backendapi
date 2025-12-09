using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.User.Commands.UpdateProfile;

public record UpdateUserProfileCommand(int userId, UserProfileDto UpdateProfileDto, bool IsSuperAdmin) : IRequest<bool>;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, bool>
{
    private readonly IUsersRepository _userRepository;
    private readonly ICryptoService _cryptoService;
    private readonly INotificationsRepository _notificationsRepository;

    public UpdateUserProfileCommandHandler(IUsersRepository userRepository, ICryptoService cryptoService, INotificationsRepository notificationsRepository)
    {
        _userRepository = userRepository;
        _cryptoService = cryptoService;
        _notificationsRepository = notificationsRepository;
    }

    public async Task<bool> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var result = await _userRepository.UpdateUserProfileAsync(
            request.UpdateProfileDto,
            _cryptoService.GenerateIntegerHashForString(request.UpdateProfileDto.EmailId),
            request.IsSuperAdmin
        );

        if (result)
        {
            var notification = new NotificationDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = request.userId,
                UpdatedBy = request.userId,
                Title = "Profile Updated",
                Message = $"Your profile is updated.",
                NotificationType = request.IsSuperAdmin ? "Super Admin" : "Info",
                IsRead = false,
                IsDeleted = false,
                UserId = request.userId,
                IsActive = false
            };
            await _notificationsRepository.InsertNotificationAsync(notification);
        }

        return result;
    }
}
