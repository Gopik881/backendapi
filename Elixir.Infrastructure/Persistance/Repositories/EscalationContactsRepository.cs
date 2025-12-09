using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using Elixir.Infrastructure.Data;

namespace Elixir.Infrastructure.Persistance.Repositories;

public class EscalationContactsRepository : IEscalationContactsRepository
{
    private readonly ElixirHRDbContext _dbContext;

    public EscalationContactsRepository(ElixirHRDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> Company5TabApproveEscalationContactsDataAsync(int companyId, List<Company5TabEscalationContactDto> escalationContact, int userId, CancellationToken cancellationToken = default)
    {

        var newContacts = escalationContact.Select(ec => new EscalationContact
        {
            CompanyId = companyId,
            FirstName = ec.FirstName,
            LastName = ec.LastName,
            Email = ec.EmailId,
            TelephoneCodeId = ec.TelephoneCodeId,
            PhoneNumber = ec.PhoneNumber,
            Designation = ec.Designation,
            Department = ec.Department,
            CreatedBy = userId,
            CreatedAt = DateTime.UtcNow,
        }).ToList();

        _dbContext.EscalationContacts.AddRange(newContacts);
        return await _dbContext.SaveChangesAsync(cancellationToken) > 0;
    }
}
