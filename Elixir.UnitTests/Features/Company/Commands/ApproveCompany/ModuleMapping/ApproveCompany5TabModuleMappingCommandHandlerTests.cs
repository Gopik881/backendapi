using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.Commands.ApproveCompany.ModuleMapping;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;

public class ApproveCompany5TabModuleMappingCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsTrue_WhenRepositoryReturnsTrue()
    {
        var repoMock = new Mock<IModuleMappingRepository>();
        repoMock.Setup(r => r.Company5TabApproveModuleMappingDataAsync(
            It.IsAny<int>(), It.IsAny<List<Company5TabModuleMappingDto>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new ApproveCompany5TabModuleMappingCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabModuleMappingCommand(1, 2, new List<Company5TabModuleMappingDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenRepositoryReturnsFalse()
    {
        var repoMock = new Mock<IModuleMappingRepository>();
        repoMock.Setup(r => r.Company5TabApproveModuleMappingDataAsync(
            It.IsAny<int>(), It.IsAny<List<Company5TabModuleMappingDto>>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var handler = new ApproveCompany5TabModuleMappingCommandHandler(repoMock.Object);
        var command = new ApproveCompany5TabModuleMappingCommand(1, 2, new List<Company5TabModuleMappingDto>());

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.False(result);
    }
}