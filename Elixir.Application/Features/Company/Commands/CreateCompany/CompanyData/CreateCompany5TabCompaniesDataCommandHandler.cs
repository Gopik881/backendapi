using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.CreateCompany.CompanyData;
public record CreateCompany5TabCommand(int userId, Company5TabCompanyDto CreateCompany5TabDto, int companyStoreageGB, int perUserStorageGB) : IRequest<bool>;
public class CreateCompany5TabCompaniesDataCommandHandler : IRequestHandler<CreateCompany5TabCommand, bool>
{
    private readonly ICompanyHistoryRepository _companyHistoryRepository;
    public CreateCompany5TabCompaniesDataCommandHandler(ICompanyHistoryRepository companyHistoryRepository)
    {
        _companyHistoryRepository = companyHistoryRepository;
    }
    public async Task<bool> Handle(CreateCompany5TabCommand request, CancellationToken cancellationToken)
    {
        // Save CompanyData
        return await _companyHistoryRepository.Company5TabCreateCompanyDataAsync(
            request.userId,
            request.CreateCompany5TabDto,
            request.companyStoreageGB,
            request.perUserStorageGB,// Pass the correct third argument as per the method signature
            cancellationToken // Pass the CancellationToken as the fourth argument
        );
    }
}


