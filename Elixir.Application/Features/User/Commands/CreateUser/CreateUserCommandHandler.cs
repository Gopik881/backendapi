using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

public record CreateUserCommand(UserProfileDto UserProfileDto) : IRequest<bool>;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, bool>
{
    private readonly IUsersRepository _userRepository;
    private readonly ICryptoService _cryptoService;
    private readonly INotificationsRepository _notificationsRepository;
    private readonly ISuperUsersRepository _superUsersRepository;
    public CreateUserCommandHandler(IUsersRepository userRepository, ICryptoService cryptoService, INotificationsRepository notificationsRepository, ISuperUsersRepository superUsersRepository)
    {
        _userRepository = userRepository;
        _cryptoService = cryptoService;
        _notificationsRepository = notificationsRepository;
        _superUsersRepository = superUsersRepository;
    }

    public async Task<bool> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Normalize email
        request.UserProfileDto.EmailId = request.UserProfileDto.EmailId.Trim().ToLowerInvariant();
        var emailHash = _cryptoService.GenerateIntegerHashForString(request.UserProfileDto.EmailId);
        // Check if email exists in superusers table
        var isSuperAdmin = await _superUsersRepository.GetUserByEmailAsync(request.UserProfileDto.EmailId, _cryptoService.GenerateIntegerHashForString(request.UserProfileDto.EmailId));
        if (isSuperAdmin != null)
        {
            // Do not allow creation if email is a superadmin
            throw new Exception("A user with this email address is a super administrator and cannot be created as a regular user.");
        }

        // Check if email already exists
       
        var exists = await _userRepository.ExistsUserByEmailAsync(request.UserProfileDto.EmailId, emailHash);
        if (exists) return false;

        // Save user data
        var result = await _userRepository.CreateUserAsync(request.UserProfileDto, emailHash);
        if (result < 0) return false;

        // Send notification if user is created successfully
        var notification = new NotificationDto
        {
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = request.UserProfileDto.UserId,
            UpdatedBy = request.UserProfileDto.UserId,
            Title = "UserCreation",
            Message = $"User '{request.UserProfileDto.FirstName} {request.UserProfileDto.LastName}' has been created successfully.",
            NotificationType = "Info",
            IsRead = false,
            IsDeleted = false,
            UserId = result,
            IsActive = false
        };
        return await _notificationsRepository.InsertNotificationAsync(notification);
    }

}
