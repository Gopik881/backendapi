using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class SuperUsersRepository : ISuperUsersRepository
{
    private readonly ElixirHRDbContext _dbContext;
    public SuperUsersRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<UserLoginDto> GetUserByEmailAsync(string email, int emailHash)
    {
        return await _dbContext.SuperUsers.Where(x => x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false)).Select(u => new UserLoginDto
        {
            Id = u.Id,
            Email = email,
            FirstName = u.FirstName,
            LastName = u.LastName,
            PasswordHash = u.PasswordHash,
            Salt = u.Salt,
            ProfilePicture = u.ProfilePicture,
            LastSessionActiveUntil = u.LastSessionActiveUntil
        }).FirstOrDefaultAsync(); 
    }

    public async Task<UserProfileDto> GetUserProfileAsync(string email, int emailHash)
    {
        return await _dbContext.SuperUsers.Where(x => x.EmailHash == emailHash && x.Email == email && x.EmailHash == emailHash && !x.IsDeleted && (x.IsEnabled ?? false))
            .Select(u => new UserProfileDto
            {
                UserId = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailId = u.Email,
                PhoneShortCutCode = _dbContext.TelephoneCodeMasters
                    .Where(t => t.Id == u.TelephoneCodeId)
                    .Select(t => t.TelephoneCode)
                    .FirstOrDefault(),
                PhoneNo = u.PhoneNumber,
                EmployeeLocation = u.Location,
                Designation = u.Designation,
                ProfilePicURL = u.ProfilePicture,
                TelePhoneCodeId = u.TelephoneCodeId,
            })
        .FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateSuperUserSessionActiveTimeAsync(string email, int emailHash, DateTime userSuperSessionActiveTime)
    {
        var user = await _dbContext.SuperUsers.FirstOrDefaultAsync(u => u.Email == email && u.EmailHash == emailHash && !u.IsDeleted && (u.IsEnabled ?? false));
        if (user == null) return false;
        user.LastSessionActiveUntil = userSuperSessionActiveTime;
        _dbContext.SuperUsers.Update(user);
        return await _dbContext.SaveChangesAsync() > 0 ? true : false;
    }
    public async Task<string?> GetSuperUserEmailAsync(int superUserId)
    {
        return await _dbContext.SuperUsers
            .Where(u => u.Id == superUserId && !u.IsDeleted)
            .Select(u => u.Email)
            .FirstOrDefaultAsync();
    }

}
