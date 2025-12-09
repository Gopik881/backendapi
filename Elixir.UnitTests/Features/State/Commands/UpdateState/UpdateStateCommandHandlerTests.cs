using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.State.Commands.UpdateState;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Moq;
using Xunit;

public class UpdateStateCommandHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsFalse_WhenStateNotFound()
    {
        var stateRepo = new Mock<IStateMasterRepository>();
        var countryRepo = new Mock<ICountryMasterRepository>();
        var entityReferenceService = new Mock<IEntityReferenceService>();
        stateRepo.Setup(r => r.GetStateByIdAsync(It.IsAny<int>())).ReturnsAsync((StateMaster)null);

        var handler = new UpdateStateCommandHandler(stateRepo.Object, countryRepo.Object, entityReferenceService.Object);
        var result = await handler.Handle(new UpdateStateCommand(1, new CreateUpdateStateDto()), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ReturnsFalse_WhenCountryNotFound()
    {
        var stateRepo = new Mock<IStateMasterRepository>();
        var countryRepo = new Mock<ICountryMasterRepository>();
        var entityReferenceService = new Mock<IEntityReferenceService>();
        stateRepo.Setup(r => r.GetStateByIdAsync(It.IsAny<int>())).ReturnsAsync(new StateMaster());
        countryRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        var handler = new UpdateStateCommandHandler(stateRepo.Object, countryRepo.Object, entityReferenceService.Object);
        var result = await handler.Handle(new UpdateStateCommand(1, new CreateUpdateStateDto()), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_UpdatesStateAndReturnsResult()
    {
        var stateRepo = new Mock<IStateMasterRepository>();
        var countryRepo = new Mock<ICountryMasterRepository>();
        var entityReferenceService = new Mock<IEntityReferenceService>();
        var state = new StateMaster();
        stateRepo.Setup(r => r.GetStateByIdAsync(It.IsAny<int>())).ReturnsAsync(state);
        countryRepo.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        stateRepo.Setup(r => r.UpdateStateAsync(state)).ReturnsAsync(true);

        var handler = new UpdateStateCommandHandler(stateRepo.Object, countryRepo.Object, entityReferenceService.Object);
        var dto = new CreateUpdateStateDto { CountryId = 1, StateName = "Test", StateShortName = "T", Description = "Desc" };
        var result = await handler.Handle(new UpdateStateCommand(1, dto), CancellationToken.None);

        Assert.True(result);
        Assert.Equal(1, state.CountryId);
        Assert.Equal("Test", state.StateName);
        Assert.Equal("T", state.StateShortName);
        Assert.Equal("Desc", state.Description);
        stateRepo.Verify(r => r.UpdateStateAsync(state), Times.Once);
    }
}