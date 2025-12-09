using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.UpdateCompany;

public record UpdateCompanyCommand(int companyId, int userId, CreateCompanyDto EditCompanyDto) : IRequest<bool>;

public class UpdateCompanyCommandHandler : IRequestHandler<UpdateCompanyCommand, bool>
{
    private readonly ICompaniesRepository _companyRepository;
    private readonly INotificationsRepository _notificationsRepostory;
    private readonly IUsersRepository _usersRepository;

    public UpdateCompanyCommandHandler(ICompaniesRepository companyRepository, INotificationsRepository notificationsRepository, IUsersRepository usersRepository)
    {
        _companyRepository = companyRepository;
        _notificationsRepostory = notificationsRepository;
        _usersRepository = usersRepository;
    }

    public async Task<bool> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        bool IsCompanyExist = await _companyRepository.ExistsWithCompanyNameForUpdateAsync(request.EditCompanyDto.CompanyName, request.companyId);
        if (IsCompanyExist) return false;

        // Update company data
        await _companyRepository.UpdateCompanyAsync(request.companyId, request.userId, request.EditCompanyDto);

        // Fetch the username for the notification message
        var userDetails = await _usersRepository.GetUserProfileByUserIdAsync(request.userId);
        string updatedByUserName = userDetails?.FirstName ?? "Unknown User";

        // Fetch all user IDs for account managers and checkers associated with the updated company
        var accountManagerAndCheckerUserIds = await _usersRepository.GetAccountManagersAndCheckersUserIdsAsync(request.companyId);

        // Send a notification to each account manager and checker
        foreach (var userId in accountManagerAndCheckerUserIds)
        {
            var notification = new NotificationDto
            {
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Title = "Company Updated",
                Message = $"The company '{request.EditCompanyDto.CompanyName}' has been updated by {updatedByUserName}.",
                UserId = userId,
                CompanyId = request.companyId,
                NotificationType = "Info",
                IsRead = false,
                IsDeleted = false,
                IsActive = false // Notification is not active by default
            };
            await _notificationsRepostory.InsertNotificationAsync(notification);

        }
        return true;
    }
}