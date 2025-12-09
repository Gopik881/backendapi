using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.Company.Commands.ApproveCompany.CompositeApproveCompany5Tab;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Notification.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class CompositeApproveCompany5TabCommandHandlerTests
{
    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenAllStepsSucceed()
    {
        var transactionMock = new Mock<ITransactionRepository>();
        transactionMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionMock.Setup(x => x.CommitAsync()).Returns(Task.CompletedTask);

        var transactionFactory = new Func<ITransactionRepository>(() => transactionMock.Object);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var notificationsRepoMock = new Mock<INotificationsRepository>();
        notificationsRepoMock.Setup(x => x.InsertNotificationAsync(It.IsAny<NotificationDto>())).ReturnsAsync(true);

        var companiesRepoMock = new Mock<ICompaniesRepository>();
        companiesRepoMock.Setup(x => x.GetCompanyByIdAsync(It.IsAny<int>())).ReturnsAsync(new Company { CompanyName = "TestCo" });

        var onboardingRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
        onboardingRepoMock.Setup(x => x.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("APPROVED");

        var usersRepoMock = new Mock<IUsersRepository>();
        usersRepoMock.Setup(x => x.GetAccountManagersAndCheckersUserIdsAsync(It.IsAny<int>())).ReturnsAsync(new List<int> { 1, 2 });

        var dbConfigSettingsMock = new Mock<IOptions<DBConfigSettings>>();
        dbConfigSettingsMock.Setup(x => x.Value).Returns(new DBConfigSettings());

        var handler = new CompositeApproveCompany5TabCommandHandler(
            transactionFactory,
            mediatorMock.Object,
            Mock.Of<ICryptoService>(),
            notificationsRepoMock.Object,
            companiesRepoMock.Object,
            onboardingRepoMock.Object,
            usersRepoMock.Object,
            dbConfigSettingsMock.Object
        );

        var dto = new Company5TabDto
        {
            Company5TabAccountDto = new Company5TabAccountDto(),
            Company5TabCompanyDto = new Company5TabCompanyDto(),
            Company5TabCompanyAdminDto = new Company5TabCompanyAdminDto(),
            Company5TabModuleMappingDto = new List<Company5TabModuleMappingDto>(),
            Company5TabReportingToolLimitsDto = new Company5TabReportingToolLimitsDto(),
            Company5TabEscalationContactDto = new List<Company5TabEscalationContactDto>(),
            company5TabElixirUserDto = new List<Company5TabElixirUserDto>()
        };

        var command = new CompositeApproveCompany5TabCommand(1, 2, dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenAnyStepFails()
    {
        var transactionMock = new Mock<ITransactionRepository>();
        transactionMock.Setup(x => x.BeginTransactionAsync()).Returns(Task.CompletedTask);
        transactionMock.Setup(x => x.RollbackAsync()).Returns(Task.CompletedTask);

        var transactionFactory = new Func<ITransactionRepository>(() => transactionMock.Object);

        var mediatorMock = new Mock<IMediator>();
        mediatorMock.Setup(x => x.Send(It.IsAny<IRequest<bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var dbConfigSettingsMock = new Mock<IOptions<DBConfigSettings>>();
        dbConfigSettingsMock.Setup(x => x.Value).Returns(new DBConfigSettings());

        var handler = new CompositeApproveCompany5TabCommandHandler(
            transactionFactory,
            mediatorMock.Object,
            Mock.Of<ICryptoService>(),
            Mock.Of<INotificationsRepository>(),
            Mock.Of<ICompaniesRepository>(),
            Mock.Of<ICompanyOnboardingStatusRepository>(),
            Mock.Of<IUsersRepository>(),
            dbConfigSettingsMock.Object
        );

        var dto = new Company5TabDto
        {
            Company5TabAccountDto = new Company5TabAccountDto(),
            Company5TabCompanyDto = new Company5TabCompanyDto(),
            Company5TabCompanyAdminDto = new Company5TabCompanyAdminDto(),
            Company5TabModuleMappingDto = new List<Company5TabModuleMappingDto>(),
            Company5TabReportingToolLimitsDto = new Company5TabReportingToolLimitsDto(),
            Company5TabEscalationContactDto = new List<Company5TabEscalationContactDto>(),
            company5TabElixirUserDto = new List<Company5TabElixirUserDto>()
        };

        var command = new CompositeApproveCompany5TabCommand(1, 2, dto);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}