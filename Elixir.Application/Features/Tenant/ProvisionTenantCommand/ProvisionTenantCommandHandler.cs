using Elixir.Application.Interfaces.Persistance;
using MediatR;
using System.Reflection.Metadata.Ecma335;

namespace Elixir.Application.Features.Tenant.ProvisionTenantCommand;

public record ProvisionTenantCommand(string TenantCode) : IRequest<bool>;
public class ProvisionTenantCommandHandler : IRequestHandler<ProvisionTenantCommand, bool>
{
    private readonly ICompaniesRepository _companyRepository;

    public ProvisionTenantCommandHandler(ICompaniesRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }


    public Task<bool> Handle(ProvisionTenantCommand request, CancellationToken cancellationToken)
    {
        if(request == null || string.IsNullOrWhiteSpace(request.TenantCode)) {
            return Task.FromResult(false);
        }

        

        throw new NotImplementedException();
    }
}
