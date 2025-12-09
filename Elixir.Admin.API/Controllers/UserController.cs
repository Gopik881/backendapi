using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Models;
using Elixir.Application.Features.User.Commands.ChangePassword;
using Elixir.Application.Features.User.Commands.DeleteUser;
using Elixir.Application.Features.User.Commands.Login;
using Elixir.Application.Features.User.Commands.ResetPassword;
using Elixir.Application.Features.User.Commands.UpdateProfile;
using Elixir.Application.Features.User.Commands.UpdateUser;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Features.User.Queries.CheckEmail;
using Elixir.Application.Features.User.Queries.GetMyProfile;
using Elixir.Application.Features.User.Queries.GetPagedNonAdminUsers;
using Elixir.Application.Features.User.Queries.GetUserCriticalGroups;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Elixir.Admin.API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class UserController : ControllerBase
    //{
    //}

    [Route("api/v{version:apiVersion}/users")]
    [ApiController]
    [ApiVersion("1.0")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IValidator<LoginCommand> _loginValidator;
        private readonly IValidator<CheckEmailExistsQuery> _emailValidator;
        private readonly IValidator<ChangePasswordCommand> _changePasswordValidator;
        private readonly IValidator<UpdateUserProfileCommand> _updateProfileValidator;
        private readonly IValidator<CreateUserCommand> _createUserValidator;
        private readonly IValidator<UpdateUserCommand> _updateUserValidator;
        private readonly IUsersRepository _usersRepository;
        private readonly ICryptoService _cryptoService;

        public UserController(
            IMediator mediator,
            IValidator<LoginCommand> loginValidator,
            IValidator<CheckEmailExistsQuery> emailValidator,
            IValidator<ChangePasswordCommand> changePasswordValidator,
            IValidator<UpdateUserProfileCommand> updateProfileValidator,
            IValidator<CreateUserCommand> createUserValidator,
            IValidator<UpdateUserCommand> updateUserValidator,
            IUsersRepository usersRepository,
            ICryptoService cryptoService)
        {
            _mediator = mediator;
           // _loginValidator = loginValidator;
            _emailValidator = emailValidator;
            _changePasswordValidator = changePasswordValidator;
            _updateProfileValidator = updateProfileValidator;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _usersRepository = usersRepository;
            _cryptoService = cryptoService;
        }

        
      // login 
      
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(int version, LoginRequestDto loginRequestDto, IMediator mediator, IValidator<LoginCommand> validator)
        {
            var command = new LoginCommand(loginRequestDto);
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var result = await mediator.Send(command);
            if (!result.Success)
                return Unauthorized(new ApiResponse<string>(401, AppConstants.ErrorCodes.INVALID_CREDENTIALS, false, result.Message));

            return Ok(new ApiResponse<LoginResponseDto>(200, AppConstants.ErrorCodes.LOGIN_SUCCESSFUL, true, result));
        }

        [HttpGet("{email}/exists")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var query = new CheckEmailExistsQuery(email);
            var validationResult = await _emailValidator.ValidateAsync(query);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var exists = await _mediator.Send(query);
            if (!exists)
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.EMAIL_NOT_FOUND, false, string.Empty));

            var emailHash = _cryptoService.GenerateIntegerHashForString(email);
            var user = await _usersRepository.GetUserByEmailAsync(email, emailHash);
            if (user == null)
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));

            return Ok(new ApiResponse<int>(200, AppConstants.ErrorCodes.EMAIL_EXIST, true, user.Id));
        }

        [HttpPut("password/change")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
        {
            var command = new ChangePasswordCommand(changePasswordRequestDto);
            var validationResult = await _changePasswordValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return Conflict(new ApiResponse<IDictionary<string, string[]>>(409, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var result = await _mediator.Send(command);
            if (result.Errors != null && result.Errors.Count != 0 || !result.Status)
                return BadRequest(new ApiResponse<ChangePasswordResponseDto>(400, AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED, result.Status, result));

            return Ok(new ApiResponse<string>(200, AppConstants.ErrorCodes.PASSWORD_CHANGE_SUCCESSFUL, result.Status, string.Empty));
        }

        [HttpPut("password/reset")]
        [Authorize]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordRequestDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(AppConstants.USER_ID)?.Value);
            var command = new ResetPasswordCommand(userId, resetPasswordRequestDto);
            var result = await _mediator.Send(command);
            if (result.Errors != null && result.Errors.Count != 0 || !result.Status)
                return BadRequest(new ApiResponse<ResetPasswordResponseDto>(400, AppConstants.ErrorCodes.PASSWORD_CHANGE_FAILED, result.Status, result));

            return Ok(new ApiResponse<string>(200, AppConstants.ErrorCodes.PASSWORD_CHANGE_SUCCESSFUL, result.Status, string.Empty));
        }

        [HttpGet("non-admin/{pageNumber:int}/{pageSize:int}")]
        [Authorize]
        public async Task<IActionResult> GetNonAdminUsers([FromQuery] string? searchTerm, int pageNumber, int pageSize)
        {
            searchTerm = searchTerm?.Trim() ?? string.Empty;
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 1 : pageSize;
            var query = new GetPagedNonAdminUsersQuery(searchTerm, pageNumber, pageSize);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_GROUP_USERS_USER_NOT_FOUND, false, string.Empty));

            return Ok(new ApiResponse<PaginatedResponse<NonAdminUserDto>>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        }

        [HttpGet("profile/{emailId}")]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(string emailId)
        {
            var isSuperAdmin = string.Equals(User.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            var query = new GetUserProfileQuery(emailId, isSuperAdmin);
            var result = await _mediator.Send(query);
            if (result == null)
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.USER_PROFILE_NOTFOUND, false, string.Empty));

            return Ok(new ApiResponse<UserProfileDto>(200, AppConstants.ErrorCodes.USER_DETAILS_FETCHED_SUCCESSFULLY, true, result));
        }

        [HttpPut("profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto updateProfileDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(AppConstants.USER_ID)?.Value);
            var isSuperAdmin = string.Equals(User.FindFirst(AppConstants.IS_SUPER_ADMIN)?.Value, AppConstants.IS_SUPER_ADMIN_TRUE);
            var command = new UpdateUserProfileCommand(userId, updateProfileDto, isSuperAdmin);
            var validationResult = await _updateProfileValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var result = await _mediator.Send(command);
            if (!result)
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.PROFILE_NOT_FOUND, false, string.Empty));

            return Ok(new ApiResponse<bool>(200, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] UserProfileDto userProfileDto)
        {
            var command = new CreateUserCommand(userProfileDto);
            var validationResult = await _createUserValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var result = await _mediator.Send(command);
            if (!result)
                return Conflict(new ApiResponse<string>(409, AppConstants.ErrorCodes.EMAIL_EXIST, false, string.Empty));

            return Created(string.Empty, new ApiResponse<bool>(201, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromBody] UserProfileDto updateUserDto)
        {
            var userId = Convert.ToInt32(User.FindFirst(AppConstants.USER_ID)?.Value);
            var command = new UpdateUserCommand(userId, updateUserDto);
            var validationResult = await _updateUserValidator.ValidateAsync(command);
            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse<IDictionary<string, string[]>>(400, AppConstants.ErrorCodes.VALIDATION_FAILED, false, validationResult.ToDictionary()));

            var result = await _mediator.Send(command);
            if (!result)
                return Conflict(new ApiResponse<string>(409, AppConstants.ErrorCodes.EMAIL_EXIST, false, string.Empty));

            return NoContent();
        }

        [HttpDelete("{userId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            var command = new DeleteUserCommand(userId);
            var result = await _mediator.Send(command);
            if (!result)
                return BadRequest(new ApiResponse<bool>(400, AppConstants.ErrorCodes.COMMON_MASTER_LIST_LINKED_ITEM_DELETE_ATTEMPT, false, false));

            return Ok(new ApiResponse<bool>(200, AppConstants.ErrorCodes.USER_MAPPING_UPDATE_SUCCESS, true, true));
        }

        [HttpGet("{userId:int}/criticalgroups")]
        [Authorize]
        public async Task<IActionResult> GetUserCriticalGroups(int userId)
        {
            var query = new GetUserCriticalGroupsQuery(userId);
            var result = await _mediator.Send(query);
            if (result == null || !result.Any())
                return NotFound(new ApiResponse<string>(404, AppConstants.ErrorCodes.CRITICAL_GROUPS_NOT_FOUND, false, string.Empty));

            return Ok(new ApiResponse<List<string>>(200, AppConstants.ErrorCodes.USER_GROUPS_FETCHED_SUCCESSFULLY, true, result));
        }
    }
}
