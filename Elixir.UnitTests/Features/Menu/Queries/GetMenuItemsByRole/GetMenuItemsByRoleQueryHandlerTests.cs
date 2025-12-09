using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Elixir.Application.Features.Menu.DTOs;
using Elixir.Application.Features.Menu.Queries.GetMenuItemsByRole;
using Elixir.Application.Interfaces.Persistance;
using Moq;
using Xunit;

namespace Elixir.UnitTests.Features.Menu.Queries.GetMenuItemsByRole
{
    public class GetMenuItemsByRoleQueryHandlerTests
    {
        private readonly Mock<IMenuItemsRepository> _menuItemsRepoMock;
        private readonly Mock<IUserGroupMappingsRepository> _userGroupMappingsRepoMock;
        private readonly GetMenuItemsByRoleQueryHandler _handler;

        public GetMenuItemsByRoleQueryHandlerTests()
        {
            _menuItemsRepoMock = new Mock<IMenuItemsRepository>();
            _userGroupMappingsRepoMock = new Mock<IUserGroupMappingsRepository>();
            _handler = new GetMenuItemsByRoleQueryHandler(_menuItemsRepoMock.Object, _userGroupMappingsRepoMock.Object);
        }

        [Fact]
        public async Task Handle_SuperUser_ReturnsMenuItems()
        {
            // Arrange
            var expected = new List<MenuItemDto> { new MenuItemDto(), new MenuItemDto() };
            _menuItemsRepoMock
                .Setup(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(0, true))
                .ReturnsAsync(expected);

            var query = new GetMenuItemsByRoleQuery(userId: 123, ISuperUser: true);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            _menuItemsRepoMock.Verify(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(0, true), Times.Once);
            _userGroupMappingsRepoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task Handle_CustomGroupUser_ReturnsMenuItems()
        {
            // Arrange
            int userId = 10;
            int customGroupId = 5;
            var expected = new List<MenuItemDto> { new MenuItemDto() };

            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(customGroupId);
            _menuItemsRepoMock
                .Setup(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(customGroupId, false))
                .ReturnsAsync(expected);

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            _userGroupMappingsRepoMock.Verify(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId), Times.Once);
            _userGroupMappingsRepoMock.Verify(r => r.GetUserDefaultUserGroupUserForMenuListingAsync(It.IsAny<int>()), Times.Never);
            _menuItemsRepoMock.Verify(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(customGroupId, false), Times.Once);
        }

        [Fact]
        public async Task Handle_DefaultGroupUser_ReturnsMenuItems()
        {
            // Arrange
            int userId = 20;
            int customGroupId = 0;
            int defaultGroupId = 7;
            var expected = new List<MenuItemDto> { new MenuItemDto() };

            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(customGroupId);
            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserDefaultUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(defaultGroupId);
            _menuItemsRepoMock
                .Setup(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(defaultGroupId, false))
                .ReturnsAsync(expected);

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Equal(expected, result);
            _userGroupMappingsRepoMock.Verify(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId), Times.Once);
            _userGroupMappingsRepoMock.Verify(r => r.GetUserDefaultUserGroupUserForMenuListingAsync(userId), Times.Once);
            _menuItemsRepoMock.Verify(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(defaultGroupId, false), Times.Once);
        }

        [Fact]
        public async Task Handle_UserNotInAnyGroup_ReturnsNull()
        {
            // Arrange
            int userId = 30;
            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(0);
            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserDefaultUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(0);

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);
            _menuItemsRepoMock.Verify(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        }

        [Fact]
        public async Task Handle_MenuItemsRepositoryReturnsEmptyList_ReturnsEmptyList()
        {
            // Arrange
            int userId = 40;
            int customGroupId = 2;
            var expected = new List<MenuItemDto>();

            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(customGroupId);
            _menuItemsRepoMock
                .Setup(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(customGroupId, false))
                .ReturnsAsync(expected);

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task Handle_UserGroupMappingsRepositoryThrows_ThrowsException()
        {
            // Arrange
            int userId = 50;
            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ThrowsAsync(new Exception("Repository error"));

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }

        [Fact]
        public async Task Handle_MenuItemsRepositoryThrows_ThrowsException()
        {
            // Arrange
            int userId = 60;
            int customGroupId = 3;
            _userGroupMappingsRepoMock
                .Setup(r => r.GetUserCustomUserGroupUserForMenuListingAsync(userId))
                .ReturnsAsync(customGroupId);
            _menuItemsRepoMock
                .Setup(r => r.GetMenuItemWithSubMenuGroupingByUserGroup(customGroupId, false))
                .ThrowsAsync(new Exception("Menu repo error"));

            var query = new GetMenuItemsByRoleQuery(userId, false);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}