using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyData;

public record ApproveCompany5TabCommand(int userId, Company5TabCompanyDto CreateCompany5TabDto, int companyStorageGB, int perUserStorageMB) : IRequest<bool>;
public class ApproveCompany5TabCompaniesDataCommandHandler : IRequestHandler<ApproveCompany5TabCommand, bool>
{
    private readonly ICompaniesRepository _companyRepository;
    public ApproveCompany5TabCompaniesDataCommandHandler(ICompaniesRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    public async Task<bool> Handle(ApproveCompany5TabCommand request, CancellationToken cancellationToken)
    {
        //Save CompanyData
        return await _companyRepository.Company5TabApproveCompanyDataAsync(request.userId, request.CreateCompany5TabDto, 
           request.companyStorageGB, request.perUserStorageMB, cancellationToken);
    }
}
