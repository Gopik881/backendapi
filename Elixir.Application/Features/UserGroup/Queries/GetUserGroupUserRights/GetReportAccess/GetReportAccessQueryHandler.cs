using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetReportAccess;

public record GetReportAccessQuery(int userGroupId) : IRequest<List<ReportingAccessDto>>;

public class GetReportAccessQueryHandler : IRequestHandler<GetReportAccessQuery, List<ReportingAccessDto>>
{
    private readonly IReportAccessRepository _reportAccessRepository;

    public GetReportAccessQueryHandler(IReportAccessRepository reportAccessRepository)
    {
        _reportAccessRepository = reportAccessRepository;
    }

    public async Task<List<ReportingAccessDto>> Handle(GetReportAccessQuery request, CancellationToken cancellationToken)
    {
        return await _reportAccessRepository.GetReportAccessData(request.userGroupId);
    }
}
