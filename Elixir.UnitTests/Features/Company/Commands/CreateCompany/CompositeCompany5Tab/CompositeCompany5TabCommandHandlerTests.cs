using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Elixir.Application.Features.Company.Commands.CreateCompany.CompositeCompany5Tab;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

public class CompositeCompany5TabCommandHandlerTests
{
    //[Fact]
    //public async Task Handle_ReturnsTrue_WhenAllStepsSucceed()
    //{
    //    var transactionRepoMock = new Mock<ITransactionRepository>();
    //    transactionRepoMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
    //    transactionRepoMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
    //    transactionRepoMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

    //    var companiesRepoMock = new Mock<ICompaniesRepository>();
    //    companiesRepoMock.Setup(r => r.GetCompanyByIdAsync(It.IsAny<int>())).ReturnsAsync(new Elixir.Domain.Entities.Company());
    //    var usersRepoMock = new Mock<IUsersRepository>();
    //    usersRepoMock.Setup(r => r.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1 });
    //    var cryptoServiceMock = new Mock<ICryptoService>();
    //    var accountHistoryRepoMock = new Mock<IAccountHistoryRepository>();
    //    var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
    //    onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Approved");
    //    var mediatorMock = new Mock<IMediator>();
    //    mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);
    //    var notificationsRepoMock = new Mock<INotificationsRepository>();
    //    notificationsRepoMock.Setup(n => n.InsertNotificationAsync(It.IsAny<Elixir.Application.Features.Notification.DTOs.NotificationDto>())).Returns((Task<bool>)Task.CompletedTask);

    //    var handler = new CompositeCompany5TabCommandHandler(
    //        () => transactionRepoMock.Object,
    //        companiesRepoMock.Object,
    //        usersRepoMock.Object,
    //        cryptoServiceMock.Object,
    //        accountHistoryRepoMock.Object,
    //        onboardingStatusRepoMock.Object,
    //        mediatorMock.Object,
    //        notificationsRepoMock.Object
    //    );

    //    var dto = new Company5TabDto
    //    {
    //        Company5TabCompanyDto = new Company5TabCompanyDto(),
    //        Company5TabAccountDto = new Company5TabAccountDto(),
    //        Company5TabCompanyAdminDto = new Company5TabCompanyAdminDto(),
    //        Company5TabModuleMappingDto = new List<Company5TabModuleMappingDto>(),
    //        Company5TabReportingToolLimitsDto = new Company5TabReportingToolLimitsDto(),
    //        Company5TabEscalationContactDto = new List<Company5TabEscalationContactDto>(),
    //        company5TabElixirUserDto = new List<Company5TabElixirUserDto>()
    //    };

    //    var command = new CompositeCompany5TabCommand(1, 2, dto);

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result);
    //}

    [Fact]
    public async Task Handle_ReturnsFalse_WhenAnyStepFails()
    {
        var transactionRepoMock = new Mock<ITransactionRepository>();
        transactionRepoMock.Setup(t => t.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionRepoMock.Setup(t => t.CommitAsync()).Returns(Task.CompletedTask);
        transactionRepoMock.Setup(t => t.RollbackAsync()).Returns(Task.CompletedTask);

        var companiesRepoMock = new Mock<ICompaniesRepository>();
        var usersRepoMock = new Mock<IUsersRepository>();
        var cryptoServiceMock = new Mock<ICryptoService>();
        var accountHistoryRepoMock = new Mock<IAccountHistoryRepository>();
        var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
        onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Approved");
        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(m => m.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var notificationsRepoMock = new Mock<INotificationsRepository>();

        var handler = new CompositeCompany5TabCommandHandler(
            () => transactionRepoMock.Object,
            companiesRepoMock.Object,
            usersRepoMock.Object,
            cryptoServiceMock.Object,
            accountHistoryRepoMock.Object,
            onboardingStatusRepoMock.Object,
            mediatorMock.Object,
            notificationsRepoMock.Object
        );

        var dto = new Company5TabDto
        {
            Company5TabCompanyDto = new Company5TabCompanyDto(),
            Company5TabAccountDto = new Company5TabAccountDto(),
            Company5TabCompanyAdminDto = new Company5TabCompanyAdminDto(),
            Company5TabModuleMappingDto = new List<Company5TabModuleMappingDto>(),
            Company5TabReportingToolLimitsDto = new Company5TabReportingToolLimitsDto(),
            Company5TabEscalationContactDto = new List<Company5TabEscalationContactDto>(),
            company5TabElixirUserDto = new List<Company5TabElixirUserDto>()
        };

        var command = new CompositeCompany5TabCommand(1, 2, dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}