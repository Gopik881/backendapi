using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ApproveCompanyOnBoardingStatus;
using Elixir.Application.Interfaces.Persistance;

public class ApproveCompany5TabOnboardingHistoryCommandHandlerTests
{
    [Fact]
    public async Task Handle_Approves_WhenStatusIsPending()
    {
        var onboardingHistoryRepoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
        var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
        var companiesRepoMock = new Mock<ICompaniesRepository>();
        var companyHistoryRepoMock = new Mock<ICompanyHistoryRepository>();

        onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>()))
            .ReturnsAsync("Pending");
        onboardingHistoryRepoMock.Setup(r => r.Company5TabCreateOnboardingHistoryAsync(
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabOnboardingHistoryCommandHandler(
            onboardingHistoryRepoMock.Object,
            onboardingStatusRepoMock.Object,
            companiesRepoMock.Object,
            companyHistoryRepoMock.Object);

        var command = new ApproveCompany5TabOnboardingHistoryCommand(1, 2);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    //[Fact]
    //public async Task Handle_UpdatesCompanyUnderEdit_WhenStatusIsApproved()
    //{
    //    var onboardingHistoryRepoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
    //    var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
    //    var companiesRepoMock = new Mock<ICompaniesRepository>();
    //    var companyHistoryRepoMock = new Mock<ICompanyHistoryRepository>();

    //    onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>()))
    //        .ReturnsAsync("Approved");
    //    companyHistoryRepoMock.Setup(r => r.GetCompanyHistoryUserIdByCompanyId(It.IsAny<int>()))
    //        .ReturnsAsync(2);
    //    companiesRepoMock.Setup(r => r.UpdateCompanyUnderEditAsync(It.IsAny<int>(), It.IsAny<int>(), false))
    //        .Returns((Task<bool>)Task.CompletedTask);
    //    onboardingHistoryRepoMock.Setup(r => r.Company5TabCreateOnboardingHistoryAsync(
    //        It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool?>()))
    //        .ReturnsAsync(true);

    //    var handler = new ApproveCompany5TabOnboardingHistoryCommandHandler(
    //        onboardingHistoryRepoMock.Object,
    //        onboardingStatusRepoMock.Object,
    //        companiesRepoMock.Object,
    //        companyHistoryRepoMock.Object);

    //    var command = new ApproveCompany5TabOnboardingHistoryCommand(1, 2);

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result);
    //}
}