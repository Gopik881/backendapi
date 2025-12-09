using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

public record AddReportingAdminsCommand(int GroupId, List<UserGroupReportingAdmin> ReportingAdmins) : IRequest<bool>;

public class AddReportingAdminsCommandHandler : IRequestHandler<AddReportingAdminsCommand, bool>
{
    private readonly IReportingAdminRepository _reportingAdminRepository;

    public AddReportingAdminsCommandHandler(IReportingAdminRepository reportingAdminRepository)
    {
        _reportingAdminRepository = reportingAdminRepository;
    }

    public async Task<bool> Handle(AddReportingAdminsCommand request, CancellationToken cancellationToken)
    {
        return await _reportingAdminRepository.AddReportingAdminsAsync(request.GroupId, request.ReportingAdmins);
    }
}