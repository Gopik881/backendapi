using Elixir.Application.Features.Company.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Company.Commands.ApproveCompany.CompanyAdmin;
public record ApproveCompany5TabCompanyAdminCommand(int companyId, int userId, Company5TabCompanyAdminDto CreateCompanyAdmin5TabDto) : IRequest<bool>;

public class ApproveCompany5TabCompanyAdminCommandHandler : IRequestHandler<ApproveCompany5TabCompanyAdminCommand, bool>
{
    private readonly ICompanyAdminUsersRepository _companyAdminUsersRepository;
    private readonly ICryptoService _cryptoService;

    public ApproveCompany5TabCompanyAdminCommandHandler(ICompanyAdminUsersRepository companyAdminUsersRepository, ICryptoService cryptoService)
    {
        _companyAdminUsersRepository = companyAdminUsersRepository;
        _cryptoService = cryptoService;
    }

    public async Task<bool> Handle(ApproveCompany5TabCompanyAdminCommand request, CancellationToken cancellationToken)
    {
        // Save data
        request.CreateCompanyAdmin5TabDto.CompanyAdminEmailHash = _cryptoService.GenerateIntegerHashForString(request.CreateCompanyAdmin5TabDto.CompanyAdminEmailId);
        return await _companyAdminUsersRepository.Company5TabApproveCompanyAdminDataAsync(request.companyId, request.userId, request.CreateCompanyAdmin5TabDto);

    }
}