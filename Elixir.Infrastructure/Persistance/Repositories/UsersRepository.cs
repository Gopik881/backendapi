using Elixir.Application.Common.Constants;
using Elixir.Application.Common.Enums;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly ElixirHRDbContext _dbContext;
    ICryptoService _cryptoService;
    public UsersRepository(ElixirHRDbContext dbContext, ICryptoService cryptoService)
    {
        _dbContext = dbContext;
        _cryptoService = cryptoService;
    }
    public async Task<UserLoginDto> GetUserByEmailAsync(string email, int emailHash)
    {
        // Try to get from Users table first
        var userDto = await _dbContext.Users
            .Where(x => x.EmailHash == emailHash && x.Email == email && !x.IsDeleted && (x.IsEnabled ?? false))
            .Select(u => new UserLoginDto
            {
                Id = u.Id,
                Email = email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PasswordHash = u.PasswordHash,
                Salt = u.Salt,
                ProfilePicture = u.ProfilePicture,
                LastSessionActiveUntil = u.LastSessionActiveUntil,
                ResetPasswordToken = u.ResetPasswordToken,
                LastFailedAttempt = u.LastFailedAttempt,
                FailedLoginAttempts = u.FailedLoginAttempts,
                isSuperUser = false // Indicate this is a regular user
            })
            .FirstOrDefaultAsync();

        if (userDto != null)
            return userDto;

        // If not found, try SuperUser table
        var superUserDto = await _dbContext.SuperUsers
            .Where(x => x.EmailHash == emailHash && x.Email == email && !x.IsDeleted && (x.IsEnabled ?? false))
            .Select(u => new UserLoginDto
            {
                Id = u.Id,
                Email = email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                PasswordHash = u.PasswordHash,
                Salt = u.Salt, // Use Salt property if available
                ProfilePicture = u.ProfilePicture,
                LastSessionActiveUntil = u.LastSessionActiveUntil,
                isSuperUser = true, // Indicate this is a super user

            })
            .FirstOrDefaultAsync();

        return superUserDto;
    }
    public async Task<UserProfileDto> GetUserProfileAsync(string email, int emailHash)
    {
        return await _dbContext.Users.Where(x => x.EmailHash == emailHash && x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false))
            .Select(u => new UserProfileDto
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailId = u.Email,
                PhoneNo = u.PhoneNumber,
                PhoneShortCutCode = _dbContext.TelephoneCodeMasters
                    .Where(t => t.Id == u.TelephoneCodeId)
                    .Select(t => t.TelephoneCode)
                    .FirstOrDefault(),
                EmployeeLocation = u.Location,
                Designation = u.Designation,
                ProfilePicURL = u.ProfilePicture,
                CreatedOn = u.CreatedAt
            })
        .FirstOrDefaultAsync();
    }
    public async Task<bool> UpdateUserPasswordAsync(string email, int emailHash, string newPasswordHash, string salt)
    {
        var user = await _dbContext.Users
            .Where(x => x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false))
            .FirstOrDefaultAsync();

        if (user != null)
        {
            user.PasswordHash = newPasswordHash;
            user.Salt = salt;
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // If not found in Users, try SuperUsers
        var superUser = await _dbContext.SuperUsers
            .Where(x => x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false))
            .FirstOrDefaultAsync();

        if (superUser != null)
        {
            superUser.PasswordHash = newPasswordHash;
            superUser.Salt = salt;
            _dbContext.SuperUsers.Update(superUser);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<bool> UpdateUserEmailHasPasswordAsync(string email, int existingemailHash, int newemailHash, string newPasswordHash, string salt)
    {
        var user = await _dbContext.Users.Where(x => x.Email == email && x.EmailHash == existingemailHash && !x.IsDeleted && (x.IsEnabled ?? false)).FirstOrDefaultAsync();
        if (user != null)
        {
            user.PasswordHash = newPasswordHash;
            user.Salt = salt;
            user.EmailHash = newemailHash; // Update the email hash
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> ExistsUserByEmailAsync(string email, int emailHash)
    {
        // Check Users table first
        var existsInUsers = await _dbContext.Users
            .AnyAsync(x => x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false));
        if (existsInUsers)
            return true;

        // If not found, check SuperUser table
        var existsInSuperUsers = await _dbContext.SuperUsers
            .AnyAsync(x => x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false));
        return existsInSuperUsers;
    }

    // Pseudocode:
    // 1. Start with base query: Users where not deleted.
    // 2. If searchTerm provided:
    //    - Trim and check for "enabled"/"disabled" (case-insensitive). If matches, filter by IsEnabled.
    //    - Otherwise filter by FirstName, LastName, Email containing searchTerm (case-insensitive).
    // 3. Count total records after filtering (totalCount).
    // 4. Order results so latest updated records appear first; fallback to CreatedAt when UpdatedAt is null:
    //    OrderByDescending(u => u.UpdatedAt ?? u.CreatedAt).ThenByDescending(u => u.CreatedAt)
    // 5. Apply Skip/Take for pagination.
    // 6. Project to NonAdminUserDto and return (list, totalCount).
    public async Task<Tuple<List<NonAdminUserDto>, int>> GetFilteredNonAdminUsersAsync(string searchTerm, int pageNumber, int pageSize)
    {
        // Ensure valid paging
        if (pageNumber <= 0) pageNumber = 1;
        if (pageSize <= 0) pageSize = 10;

        // Base query excluding soft deleted users
        var query = _dbContext.Users.Where(u => !u.IsDeleted);

        // Apply search filters server-side to avoid loading all records
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            var lowerTerm = term.ToLower();

            // If searchTerm explicitly matches enabled/disabled, filter by status
            if (string.Equals(term, "enabled", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(term, "disabled", StringComparison.OrdinalIgnoreCase))
            {
                bool status = string.Equals(term, "enabled", StringComparison.OrdinalIgnoreCase);
                query = query.Where(u => (u.IsEnabled ?? false) == status);
            }
            else
            {
                query = query.Where(u =>
                    (u.FirstName != null && u.FirstName.ToLower().Contains(lowerTerm)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(lowerTerm)) ||
                    (u.Email != null && u.Email.ToLower().Contains(lowerTerm))
                );
            }
        }

        // Total count after filtering (before pagination)
        var totalCount = await query.CountAsync();

        // Order by UpdatedAt (newest first), fallback to CreatedAt, then by CreatedAt to keep deterministic order
        var orderedQuery = query
            .OrderByDescending(u => u.UpdatedAt ?? u.CreatedAt)
            .ThenByDescending(u => u.CreatedAt);

        // Apply pagination and projection
        var users = await orderedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new NonAdminUserDto
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Status = (u.IsEnabled ?? false),
                CreatedOn = u.CreatedAt,
            })
            .ToListAsync();

        return new Tuple<List<NonAdminUserDto>, int>(users, totalCount);
    }
    public async Task<bool> UpdateUserProfileAsync(UserProfileDto userProfile, int emailHash, bool IsSuperAdmin = false)
    {
        if (!IsSuperAdmin)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userProfile.UserId && !u.IsDeleted);
            if (user == null) return false;

            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.Email = userProfile.EmailId;
            user.PhoneNumber = userProfile.PhoneNo;
            user.Location = userProfile.EmployeeLocation;
            user.Designation = userProfile.Designation;
            user.ProfilePicture = userProfile.ProfilePicURL;
            user.EmailHash = emailHash;
            user.IsEnabled = userProfile.IsEnabled;
            user.TelephoneCodeId = userProfile.TelePhoneCodeId;

            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
        else
        {
            var user = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Id == userProfile.UserId && !u.IsDeleted);
            if (user == null) return false;

            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.Email = userProfile.EmailId;
            user.PhoneNumber = userProfile.PhoneNo;
            user.Location = userProfile.EmployeeLocation;
            user.Designation = userProfile.Designation;
            user.ProfilePicture = userProfile.ProfilePicURL;
            user.EmailHash = emailHash;
            user.TelephoneCodeId = userProfile.TelePhoneCodeId;

            _dbContext.SuperUsers.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
    public async Task<int> CreateUserAsync(UserProfileDto userDto, int emailHash)
    {
        if (userDto == null) return 0;

        var user = new User
        {
            FirstName = userDto.FirstName?.Trim(),
            LastName = userDto.LastName?.Trim(),
            Email = userDto.EmailId?.Trim(),
            PhoneNumber = userDto.PhoneNo?.Trim(),
            Location = userDto.EmployeeLocation?.Trim(),
            Designation = userDto.Designation?.Trim(),
            CreatedAt = DateTime.UtcNow,
            ProfilePicture = userDto.ProfilePicURL,
            EmailHash = emailHash,
            TelephoneCodeId = userDto.TelePhoneCodeId
        };

        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
        return user.Id; // Return the newly created user's Id
    }
    public async Task<UserProfileDto?> GetUserProfileByUserIdAsync(int userId, bool? IsSuperUser = false)
    {
        if (userId == (int)Roles.SuperAdmin)
        {
            return await _dbContext.SuperUsers
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailId = u.Email,
                    PhoneNo = u.PhoneNumber,
                    EmployeeLocation = u.Location ?? string.Empty,
                    Designation = u.Designation,
                    ProfilePicURL = u.ProfilePicture,
                    IsEnabled = u.IsEnabled,
                    TelePhoneCodeId = u.TelephoneCodeId,
                    PhoneShortCutCode = _dbContext.TelephoneCodeMasters
                        .Where(t => t.Id == u.TelephoneCodeId)
                        .Select(t => t.TelephoneCode)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }
        else
        {
            return await _dbContext.Users
                .Where(u => u.Id == userId && !u.IsDeleted)
                .Select(u => new UserProfileDto
                {
                    UserId = u.Id,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailId = u.Email,
                    PhoneNo = u.PhoneNumber,
                    EmployeeLocation = u.Location ?? string.Empty,
                    Designation = u.Designation,
                    ProfilePicURL = u.ProfilePicture,
                    IsEnabled = u.IsEnabled,
                    TelePhoneCodeId = u.TelephoneCodeId,
                    CreatedOn = u.CreatedAt,
                    PhoneShortCutCode = _dbContext.TelephoneCodeMasters
                        .Where(t => t.Id == u.TelephoneCodeId)
                        .Select(t => t.TelephoneCode)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync();
        }
    }


    public async Task<bool> UpdateUserAsync(UserProfileDto userDto, int emailHash)
    {
        if (userDto == null) return false;

        // Try Users table first
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userDto.UserId && !u.IsDeleted);
        if (user != null)
        {
            user.FirstName = userDto.FirstName?.Trim() ?? user.FirstName;
            user.LastName = userDto.LastName?.Trim() ?? user.LastName;
            user.Email = userDto.EmailId?.Trim() ?? user.Email;
            user.PhoneNumber = userDto.PhoneNo?.Trim() ?? user.PhoneNumber;
            user.Location = userDto.EmployeeLocation?.Trim() ?? user.Location;
            user.Designation = userDto.Designation?.Trim() ?? user.Designation;
            user.UpdatedAt = DateTime.UtcNow;
            user.ProfilePicture = userDto.ProfilePicURL ?? user.ProfilePicture;
            user.IsEnabled = userDto.IsEnabled;
            user.EmailHash = emailHash;
            user.TelephoneCodeId = userDto.TelePhoneCodeId;

            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // If not found in Users, try SuperUsers table
        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Id == userDto.UserId && !u.IsDeleted);
        if (superUser != null)
        {
            superUser.FirstName = userDto.FirstName?.Trim() ?? superUser.FirstName;
            superUser.LastName = userDto.LastName?.Trim() ?? superUser.LastName;
            superUser.PhoneNumber = userDto.PhoneNo?.Trim() ?? superUser.PhoneNumber;
            superUser.Location = userDto.EmployeeLocation?.Trim() ?? superUser.Location;
            superUser.Designation = userDto.Designation?.Trim() ?? superUser.Designation;
            superUser.ProfilePicture = userDto.ProfilePicURL ?? superUser.ProfilePicture;
            superUser.TelephoneCodeId = userDto.TelePhoneCodeId;

            _dbContext.SuperUsers.Update(superUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }
    public async Task<bool> DeleteUserAsync(int userId)
    {
        //// Find the user by Id
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        if (user == null) return false;
        // Soft delete: set IsDeleted flag (preferred for audit), or remove if hard delete is required
        user.IsDeleted = true;
        user.Email = $"{user.Email}_{DateTime.UtcNow:yyyyMMddHHmmss}"; // Optionally change email to avoid conflicts
        _dbContext.Users.Update(user);
        return await _dbContext.SaveChangesAsync() > 0;
    }

    //public async Task<List<string>> GetUsersCriticalGroupAsync(int userId)
    //{
    //    var user = await _dbContext.Users.FirstOrDefaultAsync(e => e.Id == userId && !e.IsDeleted);

    //    // Step 1: Get all (CompanyId, RoleId) pairs where the user is the only one in that role for the company
    //    var singleRoleUser = await _dbContext.ElixirUsers
    //        .Where(e => (e.RoleId == (int)Roles.AccountManager || e.RoleId == (int)Roles.Checker))
    //        .GroupBy(e => new { e.CompanyId, e.RoleId })
    //        .Where(g => g.Count() == 1 && g.Any(e => e.UserId == userId))
    //        .Select(g => new { g.Key.CompanyId, g.Key.RoleId })
    //        .ToListAsync();

    //    // Step 2: Get company names for those CompanyIds
    //    var companyIds = singleRoleUser.Select(sr => sr.CompanyId).Distinct().ToList();
    //    var companies = await _dbContext.Companies
    //        .Where(c => companyIds.Contains(c.Id))
    //        .Select(c => new { c.Id, c.CompanyName })
    //        .ToListAsync();

    //    // Step 3: Join and format result in memory
    //    //var result = (from sr in singleRoleUser
    //    //              join c in companies on sr.CompanyId equals c.Id
    //    //              select $"Cannot be disabled as {user.FirstName} {user.LastName} is the only {(sr.RoleId == (int)Roles.AccountManager ? AppConstants.ACCOUNTMANAGER_GROUPNAME : AppConstants.CHECKER_GROUPNAME)} for the following company: {c.CompanyName}"
    //    //     ).ToList();

    //    //var result = singleRoleUser
    //    //            .Join(
    //    //            companies,
    //    //            sr => sr.CompanyId,
    //    //            c => c.Id,
    //    //            (sr, c) =>
    //    //                $"Cannot be disabled as {user.FirstName} {user.LastName} is the only " +
    //    //                $"{(sr.RoleId == (int)Roles.AccountManager ? AppConstants.ACCOUNTMANAGER_GROUPNAME : AppConstants.CHECKER_GROUPNAME)} " +
    //    //                $"for the company: {c.CompanyName}"
    //    //            )
    //    //            .ToList();

    //    //return result;

    //    var accountManagerCompanies = singleRoleUser
    //    .Where(sr => sr.RoleId == (int)Roles.AccountManager)
    //    .Join(
    //        companies,
    //        sr => sr.CompanyId,
    //        c => c.Id,
    //        (sr, c) => c.CompanyName
    //    )
    //    .ToList();

    //    var checkerCompanies = singleRoleUser
    //        .Where(sr => sr.RoleId == (int)Roles.Checker)
    //        .Join(
    //            companies,
    //            sr => sr.CompanyId,
    //            c => c.Id,
    //            (sr, c) => c.CompanyName
    //        )
    //        .ToList();

    //    var result = new List<string>();

    //    if (user != null && accountManagerCompanies.Any())
    //    {
    //        var companyList = string.Join(", ", accountManagerCompanies);
    //        result.Add($"Cannot be disabled as {user.FirstName} {user.LastName} is the only {AppConstants.ACCOUNTMANAGER_GROUPNAME} for the following company(s): {companyList}");
    //    }

    //    if (user != null && checkerCompanies.Any())
    //    {
    //        var companyList = string.Join(", ", checkerCompanies);
    //        result.Add($"Cannot be disabled as {user.FirstName} {user.LastName} is the only {AppConstants.CHECKER_GROUPNAME} for the following company(s): {companyList}");
    //    }

    //    return result;
    //}

    public async Task<List<string>> GetUsersCriticalGroupAsync(int userId)
    {
        var result = new HashSet<string>();
        var user = await _dbContext.Users.FirstOrDefaultAsync(e => e.Id == userId && !e.IsDeleted);

        if (user == null)
            return result.ToList();

        // Check for Account Manager group (client-level)
        var amClientIds = await _dbContext.ElixirUsers
            .Where(eu => eu.UserGroupId == (int)UserGroupRoles.AccountManager && eu.UserId == userId && eu.ClientId != null)
            .Select(eu => eu.ClientId)
            .Distinct()
            .ToListAsync();

        foreach (var clientIdNullable in amClientIds)
        {
            if (!clientIdNullable.HasValue)
                continue;
            var clientId = clientIdNullable.Value;

            // Get all AMs for this client
            var amUserIdsForClient = await _dbContext.ElixirUsers
                .Where(eu => eu.UserGroupId == (int)UserGroupRoles.AccountManager && eu.ClientId == clientId)
                .Select(eu => eu.UserId)
                .ToListAsync();

            // Get enabled AMs for this client from Users table
            var enabledAmUserIdsForClient = await _dbContext.Users
                .Where(u => amUserIdsForClient.Contains(u.Id) && !u.IsDeleted && (u.IsEnabled ?? false))
                .Select(u => u.Id)
                .ToListAsync();

            // If this user is the only enabled AM for this client
            if (enabledAmUserIdsForClient.Count == 1 && enabledAmUserIdsForClient.Contains(userId))
            {
                var clientName = await _dbContext.Clients
                    .Where(c => c.Id == clientId)
                    .Select(c => c.ClientName)
                    .FirstOrDefaultAsync();

                var message = $"Cannot be disabled as {user.FirstName} {user.LastName} is the only Account Manager for the client: {clientName ?? clientId.ToString()}";
                result.Add(message);
            }
        }

        // Check for Account Manager or Checker group (company-level)
        foreach (var groupId in new[] { (int)UserGroupRoles.AccountManager, (int)UserGroupRoles.Checker })
        {
            var companyIds = await _dbContext.ElixirUsers
                .Where(eu => eu.UserGroupId == groupId && eu.UserId == userId)
                .Select(eu => eu.CompanyId)
                .Distinct()
                .ToListAsync();

            foreach (var companyId in companyIds)
            {
                var userIdsForCompany = await _dbContext.ElixirUsers
                    .Where(eu => eu.UserGroupId == groupId && eu.CompanyId == companyId)
                    .Select(eu => eu.UserId)
                    .ToListAsync();

                // Get enabled users for this company from Users table
                var enabledUserIdsForCompany = await _dbContext.Users
                    .Where(u => userIdsForCompany.Contains(u.Id) && !u.IsDeleted && (u.IsEnabled ?? false))
                    .Select(u => u.Id)
                    .ToListAsync();

                if (enabledUserIdsForCompany.Count == 1 && enabledUserIdsForCompany.Contains(userId))
                {
                    var roleName = groupId == (int)UserGroupRoles.AccountManager ? AppConstants.ACCOUNTMANAGER_GROUPNAME : AppConstants.CHECKER_GROUPNAME;
                    var companyName = await _dbContext.Companies
                        .Where(c => c.Id == companyId)
                        .Select(c => c.CompanyName)
                        .FirstOrDefaultAsync();
                    if (!String.IsNullOrEmpty(companyName))
                    {
                        var message = $"Cannot be disabled as {user.FirstName} {user.LastName} is the only {roleName} for the company: {companyName ?? companyId.ToString()}";
                        result.Add(message);
                    }
                }
            }
        }

        return result.ToList();
    }
    public async Task<List<string>> GetAllUsersEmailAsync()
    {
        return await _dbContext.Users.Where(u => !u.IsDeleted)
            .Select(u => u.Email).ToListAsync();
    }
    //Write bulk insert
    public async Task<bool> BulkInsertUsersAsync(List<UserBulkUploadDto> users)
    {
        var userEntities = users.Select(u => new User
        {
            FirstName = u.FirstName?.Trim(),
            LastName = u.LastName?.Trim(),
            Email = u.Email?.Trim(),
            EmailHash = u.EmailHash,
            PhoneNumber = u.TelephonePhoneNumber?.Trim(),
            Location = u.Location?.Trim(),
            Designation = u.Designation?.Trim(),
            TelephoneCodeId = u.TelephoneCodeId,
            CreatedAt = DateTime.UtcNow,
            IsEnabled = true // Assuming new users are enabled by default
        }).ToList();
        await _dbContext.Users.AddRangeAsync(userEntities);
        await _dbContext.SaveChangesAsync();
        return true;
    }
    public async Task<List<int>> GetAccountManagersAndCheckersUserIdsAsync(int companyId)
    {
        return await _dbContext.ElixirUsers
            .Where(e =>
                e.CompanyId == companyId &&
                (
                    (e.UserGroupId == (int)UserGroupRoles.AccountManager || // AccountManager
                    (e.UserGroupId == (int)UserGroupRoles.Checker))    // Checker
                )
            )
            .Select(e => e.UserId)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> UpdateUserSessionActiveTimeAsync(string email, int emailHash, DateTime userSessionActiveTime)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.EmailHash == emailHash && !u.IsDeleted && (u.IsEnabled ?? false));
        if (user != null)
        {
            user.LastSessionActiveUntil = userSessionActiveTime;
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // If not found in Users, try SuperUsers
        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Email == email && u.EmailHash == emailHash && !u.IsDeleted && (u.IsEnabled ?? false));
        if (superUser != null)
        {
            superUser.LastSessionActiveUntil = userSessionActiveTime;
            _dbContext.SuperUsers.Update(superUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }
    // Pseudocode plan:
    // 1. Add a method to set the ResetPasswordToken for a user after email is sent.
    // 2. Add a method to check if a ResetPasswordToken exists for a user before allowing password reset.

    // 1. Save token after email is sent
    public async Task<bool> SaveResetPasswordTokenAsync(int emailHash, string email, string token)
    {
        // Try Users table first
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.EmailHash == emailHash && u.Email == email && !u.IsDeleted && (u.IsEnabled ?? false));
        if (user != null)
        {
            user.ResetPasswordToken = token;
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // If not found, try SuperUsers table
        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.EmailHash == emailHash && u.Email == email && !u.IsDeleted && (u.IsEnabled ?? false));
        if (superUser != null)
        {
            superUser.ResetPasswordToken = token;
            _dbContext.SuperUsers.Update(superUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }

    public async Task<bool> SetResetPasswordTokenEmptyAsync(string emailId, string token)
    {
        // Try Users table first
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == emailId && !u.IsDeleted);
        if (user != null)
        {
            user.ResetPasswordToken = token;
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // If not found, try SuperUsers table
        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Email == emailId && !u.IsDeleted);
        if (superUser != null)
        {
            superUser.ResetPasswordToken = token;
            _dbContext.SuperUsers.Update(superUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }

    public async Task<bool> UpdateUserLoginFailedAttemptTimeAsync(string email, int emailHash, DateTime? lastFailedAttemptTime, int? FailedLoginAttempts)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email && u.EmailHash == emailHash && !u.IsDeleted && (u.IsEnabled ?? false));
        if (user != null)
        {
            user.LastFailedAttempt = lastFailedAttemptTime;
            user.FailedLoginAttempts = FailedLoginAttempts ?? 0;
            _dbContext.Users.Update(user);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        // If not found in Users, try SuperUsers
        var superUser = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Email == email && u.EmailHash == emailHash && !u.IsDeleted && (u.IsEnabled ?? false));
        if (superUser != null)
        {
            superUser.LastFailedAttempt = lastFailedAttemptTime;
            superUser.FailedLoginAttempts = FailedLoginAttempts ?? 0;
            _dbContext.SuperUsers.Update(superUser);
            return await _dbContext.SaveChangesAsync() > 0;
        }

        return false;
    }
    // PSEUDOCODE (detailed):
    // 1. Create a new helper method ValidateResetLinkTokenAsync that returns (bool IsValid, bool UserHasPassword).
    //    - Validate token is not null/empty.
    //    - Decrypt token using _cryptoService.DecryptPasswordResetData(token).
    //    - Ensure decrypted data is not null and expiry is in the future.
    //    - Extract email and emailHash from the decrypted DTO.
    //    - Lookup user in Users table by email & emailHash (and not deleted).
    //      - Ensure stored ResetPasswordToken matches provided token (guard against token reuse/mismatch).
    //      - Determine whether the user's PasswordHash is present (userHasPassword = !string.IsNullOrWhiteSpace(PasswordHash)).
    //      - Return (true, userHasPassword).
    //    - If not found, lookup SuperUsers similarly, return (true, userHasPassword) when found.
    //    - If no user found or any validation fails, return (false, false).
    //
    // 2. Keep the existing ValidateResetLinkToken(string token) method for backward compatibility:
    //    - Call the new helper and return only the IsValid boolean.
    //
    // NOTES:
    // - This approach preserves the existing method signature while providing an async helper that also
    //   indicates whether the user already has a password.
    // - Assumes PasswordResetTokenDataDto contains Email (string) and EmailHash (int) properties
    //   in addition to ExpiryDate. If property names differ, adjust accordingly.

    public async Task<(bool IsValid, bool UserHasPassword)> ValidateResetLinkTokenAsync(string token)
    {
        // Basic null/empty check
        if (string.IsNullOrEmpty(token))
            return (false, false);

        // Decrypt token and obtain token data
        var tokenData = _cryptoService.DecryptPasswordResetData(token);
        if (tokenData == null)
            return (false, false);

        // Check expiry
        if (tokenData.ExpiryDate <= DateTime.UtcNow)
            return (false, false);

        // Expect tokenData to carry Email and EmailHash - validate values
        // Adjust property names if your DTO uses different names.
        var email = (tokenData as dynamic)?.Email as string;
        //var emailHashObj = (tokenData as dynamic)?.EmailHash;
        //int emailHash = 0;
        //if (emailHashObj is int eh) emailHash = eh;
        // If email or emailHash is not available, fail validation
        if (string.IsNullOrWhiteSpace(email))
            return (false, false);

        email = email.Trim();

        // Try Users table
        var user = await _dbContext.Users
            .Where(u => u.Email == email && !u.IsDeleted)
            .Select(u => new { u.PasswordHash, u.ResetPasswordToken })
            .FirstOrDefaultAsync();

        if (user != null)
        {
            // Ensure token stored in DB matches the provided token (prevents using leaked tokens)
            if (string.IsNullOrEmpty(user.ResetPasswordToken) || user.ResetPasswordToken != token)
                return (false, false);

            bool userHasPassword = !string.IsNullOrWhiteSpace(user.PasswordHash);
            return (true, userHasPassword);
        }

        // Try SuperUsers table
        var superUser = await _dbContext.SuperUsers
            .Where(u => u.Email == email && !u.IsDeleted)
            .Select(u => new { u.PasswordHash, u.ResetPasswordToken })
            .FirstOrDefaultAsync();

        if (superUser != null)
        {
            if (string.IsNullOrEmpty(superUser.ResetPasswordToken) || superUser.ResetPasswordToken != token)
                return (false, false);

            bool userHasPassword = !string.IsNullOrWhiteSpace(superUser.PasswordHash);
            return (true, userHasPassword);
        }

        // No matching user found
        return (false, false);
    }

    public async Task<bool> ValidateResetLinkToken(string token)
    {
        var result = await ValidateResetLinkTokenAsync(token);
        return result.IsValid;
    }

}
