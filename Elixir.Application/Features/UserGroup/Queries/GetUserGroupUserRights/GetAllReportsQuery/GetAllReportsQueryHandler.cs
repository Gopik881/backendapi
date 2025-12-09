using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetAllReportsQuery;

public record GetAllReportsQuery(int UserGroupId) : IRequest<List<SelectionItemDto>>;

public class GetAllReportsQueryHandler : IRequestHandler<GetAllReportsQuery, List<SelectionItemDto>>
{
    private readonly IReportAccessRepository _reportsRepository;

    public GetAllReportsQueryHandler(IReportAccessRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<List<SelectionItemDto>> Handle(GetAllReportsQuery request, CancellationToken cancellationToken)
    {
        return await _reportsRepository.GetReportsForRoleAsync(request.UserGroupId);
    }
}
