using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.RejectCompany;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class RejectCompany5TabCommandHandlerTests
{
    [Fact]
    public async Task Handle_ApprovesCompany_UpdatesCompanyAndCreatesHistory()
    {
        var onboardingRepo = new Mock<ICompany5TabOnboardingHistoryRepository>();
        var statusRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var companiesRepo = new Mock<ICompaniesRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();
        var mediator = new Mock<MediatR.IMediator>();
        var usersRepo = new Mock<IUsersRepository>();

        statusRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Approved");
        usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1 });

        // Fix: Replace anonymous type with a concrete type that matches the expected return type of GetCompanyByIdAsync
        var company = new Company { CompanyName = "TestCo" };
        companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>())).ReturnsAsync(company);

        var handler = new RejectCompany5TabCommandHandler(
            onboardingRepo.Object, statusRepo.Object, companiesRepo.Object, notificationsRepo.Object, mediator.Object, usersRepo.Object);

        var result = await handler.Handle(new RejectCompany5TabCommand(1, 2, "reason"), CancellationToken.None);

        Assert.True(result);
        companiesRepo.Verify(r => r.UpdateCompanyUnderEditAsync(1, 2, false), Times.Once);
        onboardingRepo.Verify(r => r.Company5TabCreateOnboardingHistoryAsync(1, 2, "Rejected", "reason", true), Times.Once);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PendingCompany_UpdatesStatusAndCreatesHistory()
    {
        var onboardingRepo = new Mock<ICompany5TabOnboardingHistoryRepository>();
        var statusRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var companiesRepo = new Mock<ICompaniesRepository>();
        var notificationsRepo = new Mock<INotificationsRepository>();
        var mediator = new Mock<MediatR.IMediator>();
        var usersRepo = new Mock<IUsersRepository>();

        statusRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Pending");
        usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1 });

        // Fix: Replace anonymous type with a concrete type that matches the expected return type of GetCompanyByIdAsync
        var company = new Company { CompanyName = "TestCo" };
        companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>())).ReturnsAsync(company);

        var handler = new RejectCompany5TabCommandHandler(
            onboardingRepo.Object, statusRepo.Object, companiesRepo.Object, notificationsRepo.Object, mediator.Object, usersRepo.Object);

        var result = await handler.Handle(new RejectCompany5TabCommand(1, 2, "reason"), CancellationToken.None);

        Assert.True(result);
        //statusRepo.Verify(r => r.UpdateOnboardingStatusAsync(1, 2, "Rejected"), Times.Once);
        statusRepo.Verify(r => r.UpdateOnboardingStatusAsync(1, 2, "Rejected", It.IsAny<bool>()), Times.Once);
        onboardingRepo.Verify(r => r.Company5TabCreateOnboardingHistoryAsync(1, 2, "Rejected", "reason", false), Times.Once);
        notificationsRepo.Verify(r => r.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Once);
    }
}