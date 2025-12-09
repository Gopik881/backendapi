using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.WithdrawCompany.CompositeWithdrawCompany5Tab;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;
using Moq;
using Xunit;

public class CompositeWithDrawCompany5TabCommandHandlerTests
{
    [Fact]
    public async Task Handle_AllWithdrawsSucceed_CommitsAndReturnsTrue()
    {
        var transactionRepo = new Mock<ITransactionRepository>();
        transactionRepo.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionRepo.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);

        var mediator = new Mock<IMediator>();
        mediator.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var notificationsRepo = new Mock<INotificationsRepository>();
        var onboardingStatusRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var companiesRepo = new Mock<ICompaniesRepository>();
        var usersRepo = new Mock<IUsersRepository>();

        companiesRepo.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>())).ReturnsAsync(new Company { CompanyName = "TestCo" });
        onboardingStatusRepo.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Approved");
        usersRepo.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1 }); // Fix: Changed to List<int> instead of array

        var handler = new CompositeWithDrawCompany5TabCommandHandler(
            () => transactionRepo.Object, mediator.Object, notificationsRepo.Object, onboardingStatusRepo.Object, companiesRepo.Object, usersRepo.Object);

        var notificationDto = new NotificationDto
        {
            Id = 1,
            Message = "Test Notification",
            CreatedAt = DateTime.UtcNow
        };

        notificationsRepo.Setup(n => n.InsertNotificationAsync(It.IsAny<NotificationDto>())).ReturnsAsync(true);

        var result = await handler.Handle(new CompositeWithDrawCompany5TabCommand(1, 2), CancellationToken.None);

        Assert.True(result);
        transactionRepo.Verify(t => t.CommitAsync(), Times.Once);
        notificationsRepo.Verify(n => n.InsertNotificationAsync(It.IsAny<NotificationDto>()), Times.Once);
    }

    [Fact]
    public async Task Handle_AnyWithdrawFails_RollsBackAndReturnsFalse()
    {
        var transactionRepo = new Mock<ITransactionRepository>();
        transactionRepo.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionRepo.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var mediator = new Mock<IMediator>();
        mediator.SetupSequence(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var notificationsRepo = new Mock<INotificationsRepository>();
        var onboardingStatusRepo = new Mock<ICompanyOnboardingStatusRepository>();
        var companiesRepo = new Mock<ICompaniesRepository>();
        var usersRepo = new Mock<IUsersRepository>();

        var handler = new CompositeWithDrawCompany5TabCommandHandler(
            () => transactionRepo.Object, mediator.Object, notificationsRepo.Object, onboardingStatusRepo.Object, companiesRepo.Object, usersRepo.Object);

        var result = await handler.Handle(new CompositeWithDrawCompany5TabCommand(1, 2), CancellationToken.None);

        Assert.False(result);
        transactionRepo.Verify(t => t.RollbackAsync(), Times.Once);
    }
}