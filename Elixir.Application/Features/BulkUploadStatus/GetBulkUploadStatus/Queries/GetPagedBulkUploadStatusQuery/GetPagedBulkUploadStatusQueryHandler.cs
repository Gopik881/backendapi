using Elixir.Application.Common.DTOs;
using Elixir.Application.Common.Models;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetPagedBulkUploadStatusQuery;

public record GetPagedBulkUploadStatusQuery(Guid ProcessId, int PageNumber, int PageSize) :IRequest<PaginatedResponse<BulkUploadErrorListDto>>;

public class GetPagedBulkUploadStatusQueryHandler : IRequestHandler<GetPagedBulkUploadStatusQuery, PaginatedResponse<BulkUploadErrorListDto>>
{
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public GetPagedBulkUploadStatusQueryHandler(IBulkUploadErrorListRepository  bulkUploadErrorListRepository)
    {
        _bulkUploadErrorListRepository=bulkUploadErrorListRepository;
    }
    public async Task<PaginatedResponse<BulkUploadErrorListDto>> Handle(GetPagedBulkUploadStatusQuery request, CancellationToken cancellationToken)
    {
        var result =await _bulkUploadErrorListRepository.GetPagedBulkUploadErrorListAsync(request.ProcessId, request.PageNumber, request.PageSize);
        return new PaginatedResponse<BulkUploadErrorListDto>(result.Item1, new PaginationMetadata(result.Item2, request.PageSize, request.PageNumber));
    }
}
