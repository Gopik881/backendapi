using Elixir.Application.Features.UserGroup.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.UserGroup.Queries.GetUserGroupUserRights.GetAllCategoriesQuery;

public record GetAllCategoriesQuery(int UserGroupId) : IRequest<List<SelectionItemDto>>;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<SelectionItemDto>>
{
    private readonly IReportAccessRepository _reportsRepository;

    public GetAllCategoriesQueryHandler(IReportAccessRepository reportsRepository)
    {
        _reportsRepository = reportsRepository;
    }

    public async Task<List<SelectionItemDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _reportsRepository.GetCategoriesForReportsAsync(request.UserGroupId);
    }
}
