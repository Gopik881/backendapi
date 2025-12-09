using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Features.Company.Queries.GetCompany5TabCustomUsers;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

public class GetCompany5TabCustomUserGroupsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ReturnsUserGroups()
    {
        var repo = new Mock<ICompaniesRepository>();
        var expected = new List<UserGroupDto>
        {
            new UserGroupDto
            {
                GroupId = 1,
                GroupName = "Test Group",
                GroupType = "Custom",
                IsSuperAdminCreatedGroup = false,
                Status = true,
                Description = "Test Description"
            }
        };
        // Fix for CS0854: Avoid using optional arguments in expression trees.
        // Instead, specify all arguments explicitly in the Setup call.
        repo.Setup(r => r.GetCompany5TabCustomUserGroups(1, "")).Returns(Task.FromResult(expected));

        var handler = new GetCompany5TabCustomUserGroupsQueryHandler(repo.Object);
        var result = await handler.Handle(new GetCompany5TabCustomUserGroupsQuery(1), CancellationToken.None);

        Assert.Equal(expected.Count, result.Count);
        Assert.Equal(expected[0].GroupId, result[0].GroupId);
        Assert.Equal(expected[0].GroupName, result[0].GroupName);
        Assert.Equal(expected[0].GroupType, result[0].GroupType);
        Assert.Equal(expected[0].IsSuperAdminCreatedGroup, result[0].IsSuperAdminCreatedGroup);
        Assert.Equal(expected[0].Status, result[0].Status);
        Assert.Equal(expected[0].Description, result[0].Description);
    }
}