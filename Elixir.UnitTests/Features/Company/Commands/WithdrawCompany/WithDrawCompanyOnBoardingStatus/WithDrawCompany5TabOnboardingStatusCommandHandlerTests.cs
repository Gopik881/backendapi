using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Constants;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.WithDrawCompanyOnBoardingStatus;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class WithDrawCompany5TabOnboardingStatusCommandHandlerTests
{
    private readonly Mock<ICompanyOnboardingStatusRepository> _onboardingRepo = new();
    private readonly Mock<ICompaniesRepository> _companiesRepo = new();
    private readonly Mock<INotificationsRepository> _notificationsRepo = new();
    private readonly Mock<IUsersRepository> _usersRepo = new();

    private WithDrawCompany5TabOnboardingStatusCommandHandler CreateHandler() =>
        new(_onboardingRepo.Object, _companiesRepo.Object, _notificationsRepo.Object, _usersRepo.Object);

    //[Fact]
    //public async Task Handle_OnboardingStatusPending_UpdatesStatusAndSendsNotifications()
    //{
    //    _onboardingRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>()))
    //        .ReturnsAsync(AppConstants.ONBOARDING_STATUS_PENDING);
    //    _usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>()))
    //        .ReturnsAsync(new List<int> { 1, 2 });
    //    _companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>()))
    //        .ReturnsAsync(new Company { CompanyName = "TestCo" });

    //    var handler = CreateHandler();
    //    var result = await handler.Handle(new WithDrawCompany5TabOnboardingStatusCommand(1, 2), CancellationToken.None);

    //    Assert.True(result);
    //    _onboardingRepo.Verify(r => r.UpdateOnboardingStatusAsync(1, 2, AppConstants.ONBOARDING_STATUS_NEW), Times.Once);
    //    _notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Exactly(2));
    //}

    [Fact]
    public async Task Handle_OnboardingStatusApproved_UpdatesCompanyAndSendsNotifications()
    {
        _onboardingRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>()))
            .ReturnsAsync(AppConstants.ONBOARDING_STATUS_APPROVED);
        _usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<int> { 1 });
        _companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new Company { CompanyName = "TestCo" });

        var handler = CreateHandler();
        var result = await handler.Handle(new WithDrawCompany5TabOnboardingStatusCommand(1, 2), CancellationToken.None);

        Assert.True(result);
        _companiesRepo.Verify(r => r.UpdateCompanyUnderEditAsync(1, 2, false), Times.Once);
        _notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Once);
    }

    //[Fact]
    //public async Task Handle_UnknownOnboardingStatus_StillSendsNotifications()
    //{
    //    _onboardingRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>()))
    //        .ReturnsAsync("OtherStatus");
    //    _usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>()))
    //        .ReturnsAsync(new List<int>());
    //    _companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>()))
    //        .ReturnsAsync(new Company { CompanyName = "TestCo" });

    //    var handler = CreateHandler();
    //    var result = await handler.Handle(new WithDrawCompany5TabOnboardingStatusCommand(1, 2), CancellationToken.None);

    //    Assert.True(result);
    //    _onboardingRepo.Verify(r => r.UpdateOnboardingStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never);
    //    _companiesRepo.Verify(r => r.UpdateCompanyUnderEditAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
    //    _notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Never);
    //}
}