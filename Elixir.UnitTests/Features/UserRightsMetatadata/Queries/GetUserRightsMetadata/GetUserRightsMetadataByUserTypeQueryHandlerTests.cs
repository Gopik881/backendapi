using System;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.UserRightsMetatadata.DTOs;
using Elixir.Application.Features.UserRightsMetatadata.Queries.GetUserRightsMetadata;
using Xunit;

public class GetUserRightsMetadataByUserTypeQueryHandlerTests
{
    [Fact]
    public async Task Handle_UserType1_ReturnsUserType1Metadata()
    {
        // Arrange
        var handler = new GetUserRightsMetadataByUserTypeQueryHandler();
        var query = new GetUserRightsMetadataByUserTypeQuery(1);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UserRightsMetedataResponseDto>(result);
        // Optionally, assert specific properties if needed
        // Assert.Equal("User rights retrieved successfully.", result.Message);
    }

    [Fact]
    public async Task Handle_UserType2_ReturnsUserType2Metadata()
    {
        // Arrange
        var handler = new GetUserRightsMetadataByUserTypeQueryHandler();
        var query = new GetUserRightsMetadataByUserTypeQuery(2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UserRightsMetedataResponseDto>(result);
        // Optionally, assert specific properties if needed
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(3)]
    [InlineData(999)]
    public async Task Handle_InvalidUserType_ThrowsArgumentException(int invalidUserType)
    {
        // Arrange
        var handler = new GetUserRightsMetadataByUserTypeQueryHandler();
        var query = new GetUserRightsMetadataByUserTypeQuery(invalidUserType);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => handler.Handle(query, CancellationToken.None));
    }
}