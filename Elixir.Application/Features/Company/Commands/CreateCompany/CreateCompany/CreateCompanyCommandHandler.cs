using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.CreateCompany
{
    public record CreateCompanyCommand(int userId, CreateCompanyDto CreateCompanyDto) : IRequest<ApiResponse<bool>>;

    public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, ApiResponse<bool>>
    {
        private readonly ICompaniesRepository _companyRepository;
        private readonly INotificationsRepository _notificationsRepository;
        private readonly IUsersRepository _usersRepository;

        public CreateCompanyCommandHandler(ICompaniesRepository companyRepository, INotificationsRepository notificationsRepository, IUsersRepository usersRepository)
        {
            _companyRepository = companyRepository;
            _notificationsRepository = notificationsRepository;
            _usersRepository = usersRepository;
        }

        public async Task<ApiResponse<bool>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
        {
            var response = new ApiResponse<bool>();

            bool isCompanyExist = await _companyRepository.ExistsWithCompanyNameAsync(request.CreateCompanyDto.CompanyName);
            if (isCompanyExist)
            {
                response.StatusCode = 400;
                response.Message = AppConstants.ErrorCodes.DUPLICATE_COMPANY_NAME;//"Company name already exists.";
                response.Success = false;
                response.Data = false;
                response.Errors.Add("Duplicate company name.");
                return response;
            }

            var result = await _companyRepository.CreateCompanyAsync(request.userId, request.CreateCompanyDto);
            if (result < 0)
            {
                response.StatusCode = 500;
                response.Message = AppConstants.ErrorCodes.CLIENT_ACCOUNT_MANAGERS_RETRIEVAL_FAILED;
                response.Success = false;
                response.Data = false;
                response.Errors.Add("Database operation failed.");
                return response;
            }

            var userDetails = await _usersRepository.GetUserProfileByUserIdAsync(request.userId);
            string createdByUserName = userDetails?.FirstName ?? "Unknown User";

            var accountManagerAndCheckerUserIds = await _usersRepository.GetAccountManagersAndCheckersUserIdsAsync(result);
            bool IsSuperUser = false;
            foreach (var userId in accountManagerAndCheckerUserIds)
            {
                var notification = new NotificationDto
                {
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Title = "Company Creation",
                    Message = $"A new company '{request.CreateCompanyDto.CompanyName}' has been created by {createdByUserName}.",
                    UserId = userId,
                    CompanyId = result,
                    NotificationType = IsSuperUser ? "CreatedBySuperAdmin" : "Info",
                    IsRead = false,
                    IsDeleted = false,
                    IsActive = false
                };
                await _notificationsRepository.InsertNotificationAsync(notification);
            }

            response.StatusCode = 201;
            response.Message = "Company created successfully.";
            response.Success = true;
            response.Data = true;
            return response;
        }
    }
}
