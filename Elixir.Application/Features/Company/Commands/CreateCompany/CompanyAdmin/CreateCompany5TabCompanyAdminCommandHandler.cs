using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

public record CreateCompany5TabCompanyAdminCommand(int companyId, int userId, Company5TabCompanyAdminDto CreateCompanyAdmin5TabDto) : IRequest<bool>;

public class CreateCompany5TabCompanyAdminCommandHandler : IRequestHandler<CreateCompany5TabCompanyAdminCommand, bool>
{
    private readonly ICompanyAdminUsersHistoryRepository _companyAdminUsersHistoryRepository;
    
    public CreateCompany5TabCompanyAdminCommandHandler(ICompanyAdminUsersHistoryRepository companyAdminUsersHistoryRepository)
    {
        _companyAdminUsersHistoryRepository = companyAdminUsersHistoryRepository;
    }

    public async Task<bool> Handle(CreateCompany5TabCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        // Save data
        return await _companyAdminUsersHistoryRepository.Company5TabCreateCompanyAdminDataAsync(request.companyId, request.userId, request.CreateCompanyAdmin5TabDto);
        
    }
}
