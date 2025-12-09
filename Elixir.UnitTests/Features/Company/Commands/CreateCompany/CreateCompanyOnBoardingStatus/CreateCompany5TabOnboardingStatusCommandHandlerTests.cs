using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Interfaces.Persistance;

public class CreateCompany5TabOnboardingHistoryCommandHandlerTests
{
    //[Fact]
    //public async Task Handle_UpdatesStatusAndHistory_WhenStatusIsNewOrRejected()
    //{
    //    var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
    //    onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("New");
    //    onboardingStatusRepoMock.Setup(r => r.UpdateOnboardingStatusAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns((Task<bool>)Task.CompletedTask);

    //    var onboardingHistoryRepoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
    //    onboardingHistoryRepoMock.Setup(r => r.Company5TabCreateOnboardingHistoryAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>(), null, false)).Returns((Task<bool>)Task.CompletedTask);

    //    var companiesRepoMock = new Mock<ICompaniesRepository>();

    //    var handler = new CreateCompany5TabOnboardingHistoryCommandHandler(onboardingHistoryRepoMock.Object, onboardingStatusRepoMock.Object, companiesRepoMock.Object);
    //    var command = new CreateCompany5TabOnboardingHistoryCommand(1, 2);

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result);
    //}

    //[Fact]
    //public async Task Handle_UpdatesCompanyUnderEdit_WhenStatusIsApproved()
    //{
    //    var onboardingStatusRepoMock = new Mock<ICompanyOnboardingStatusRepository>();
    //    onboardingStatusRepoMock.Setup(r => r.GetCompanyOnBoardingStatus(It.IsAny<int>())).ReturnsAsync("Approved");

    //    var onboardingHistoryRepoMock = new Mock<ICompany5TabOnboardingHistoryRepository>();
    //    var companiesRepoMock = new Mock<ICompaniesRepository>();
    //    companiesRepoMock.Setup(r => r.UpdateCompanyUnderEditAsync(It.IsAny<int>(), It.IsAny<int>(), true)).Returns((Task<bool>)Task.CompletedTask);

    //    var handler = new CreateCompany5TabOnboardingHistoryCommandHandler(onboardingHistoryRepoMock.Object, onboardingStatusRepoMock.Object, companiesRepoMock.Object);
    //    var command = new CreateCompany5TabOnboardingHistoryCommand(1, 2);

    //    var result = await handler.Handle(command, CancellationToken.None);

    //    Assert.True(result);
    //}
}