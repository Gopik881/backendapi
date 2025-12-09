using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Features.Company.DTOs;
using Microsoft.EntityFrameworkCore;
using Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class EscalationContactsHistoryRepository : IEscalationContactsHistoryRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public EscalationContactsHistoryRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<bool> Company5TabCreateEscalationContactsDataAsync(int companyId, List<Company5TabEscalationContactDto> escalationContact, int userId, CancellationToken cancellationToken = default)
    {
        int lastVersion = _dbContext.EscalationContactsHistories
            .Where(mh => mh.CompanyId == companyId && !mh.IsDeleted)
            .OrderByDescending(mh => mh.Version)
            .Select(mh => mh.Version)
            .FirstOrDefault();

        if (escalationContact.Count == 0 || escalationContact == null)
        {
            var empayContacts = new EscalationContactsHistory
            {
                CompanyId = companyId,
                FirstName = "EMPTYRECORDS",
                LastName = "EMPTYRECORDS",
                Email = "EMPTY@yopmail.com",
                TelephoneCodeId = null,
                PhoneNumber = string.Empty,
                Designation = string.Empty,
                Department = string.Empty,
                CreatedBy = userId,
                Remarks = string.Empty,
                CreatedAt = DateTime.UtcNow,
                Version = lastVersion + 1
            };
            _dbContext.EscalationContactsHistories.AddRange(empayContacts);
            return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
        }

        // Pseudocode:
        // - When mapping or assigning TelephoneCodeId from input (ec.TelephoneCodeId), check if it is 0.
        // - If 0, set TelephoneCodeId to 1 by default.
        // - Otherwise, use the provided value.

        var newContacts = escalationContact.Select(ec => new EscalationContactsHistory
        {
            CompanyId = companyId,
            FirstName = ec.FirstName,
            LastName = ec.LastName,
            Email = ec.EmailId,
            TelephoneCodeId = (ec.TelephoneCodeId == 0) ? 1 : ec.TelephoneCodeId,
            PhoneNumber = ec.PhoneNumber,
            Designation = ec.Designation,
            Department = ec.Department,
            CreatedBy = userId,
            Remarks = ec.Remarks,
            CreatedAt = DateTime.UtcNow,
            Version = lastVersion + 1
        }).ToList();

        _dbContext.EscalationContactsHistories.AddRange(newContacts);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<List<Company5TabEscalationContactDto>?> GetCompany5TabLatestEscalationContactsHistoryAsync(int companyId, bool isPrevious, CancellationToken cancellationToken = default)
    {
        var versionQueryEscalationContacts = _dbContext.EscalationContactsHistories
            .Where(ec => ec.CompanyId == companyId && !ec.IsDeleted);

        int? latestVersion = 0;
        if (await versionQueryEscalationContacts.AnyAsync(cancellationToken))
        {
            latestVersion = isPrevious
                ? await versionQueryEscalationContacts
                    .Select(e => e.Version)
                    .Distinct()
                    .OrderByDescending(v => v)
                    .Skip(1)
                    .FirstOrDefaultAsync(cancellationToken)
                : await versionQueryEscalationContacts
                    .Select(e => e.Version)
                    .Distinct()
                    .MaxAsync(cancellationToken);
        }

        return await _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId && e.FirstName != "EMPTYRECORDS" && !e.IsDeleted && e.Version == latestVersion)
            .OrderByDescending(c => c.Version)
            .Select(e => new Company5TabEscalationContactDto
            {
                FirstName = e.FirstName,
                LastName = e.LastName,
                EmailId = e.Email,
                TelephoneCodeId = e.TelephoneCodeId,
                PhoneCode = e.TelephoneCodeId != null
                    ? (from t in _dbContext.TelephoneCodeMasters
                       where t.Id == e.TelephoneCodeId
                       select t.TelephoneCode).FirstOrDefault()
                    : null,
                PhoneNumber = e.PhoneNumber,
                Designation = e.Designation,
                Department = e.Department,
                createdBy = e.CreatedBy,
                Remarks = e.Remarks
            }).ToListAsync(cancellationToken);
    }
    public async Task<bool> WithdrawCompany5TabEscalationContactsHistoryAsync(int companyId, int userId, CancellationToken cancellationToken = default)
    {
        // Get the latest version for the given company and user
        int? latestVersion = await _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId)
            .MaxAsync(e => (int?)e.Version, cancellationToken);

        if (latestVersion == null) return true;

        // Find all records to remove
        var recordsToRemove = _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId && e.Version == latestVersion);
        if(recordsToRemove.Count() == 0) return true; // Nothing to remove
        _dbContext.EscalationContactsHistories.RemoveRange(recordsToRemove);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
    public async Task<Company5TabHistoryDto> GetCompany5TabEscalationContactsHistoryByVersionAsync(
        int userId,
        int companyId,
        int versionNumber)
    {
        // Fetch the two most recent versions: requested and previous
        var histories = await _dbContext.EscalationContactsHistories
            .Where(e => e.CompanyId == companyId && (e.Version == versionNumber || e.Version == versionNumber - 1))
            .OrderByDescending(e => e.Version)
            .ToListAsync();

        Company5TabHistoryMapEntitytoDictionary company5TabHistoryMapEntitytoDictionary = new Company5TabHistoryMapEntitytoDictionary();

        var company5TabEscalationContactsHistory = new Company5TabHistoryDto();

        foreach (var history in histories)
        {
            var mapped = company5TabHistoryMapEntitytoDictionary.MapEntityToDictionary(history)
                .ToDictionary(
                    keyValue => keyValue.Key,
                    keyValue => (object)new Dictionary<string, string> { { keyValue.Key, keyValue.Value } }
                );
            company5TabEscalationContactsHistory.Company5TabHistory[history.Version.ToString()] = mapped;
        }

        return company5TabEscalationContactsHistory;
    }

}
