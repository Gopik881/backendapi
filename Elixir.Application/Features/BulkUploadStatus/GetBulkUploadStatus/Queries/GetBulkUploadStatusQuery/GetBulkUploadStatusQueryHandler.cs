using Elixir.Application.Common.DTOs;
using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Queries.GetBulkUploadStatusQuery;

public record GetBulkUploadStatusQuery(Guid ProcessId) : IRequest<List<BulkUploadErrorListDto>>;
public class GetBulkUploadStatusQueryHandler : IRequestHandler<GetBulkUploadStatusQuery, List<BulkUploadErrorListDto>>
{
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public GetBulkUploadStatusQueryHandler(IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _bulkUploadErrorListRepository = bulkUploadErrorListRepository;
    }
    public async Task<List<BulkUploadErrorListDto>> Handle(GetBulkUploadStatusQuery request, CancellationToken cancellationToken)
    {
        return await _bulkUploadErrorListRepository.GetBulkUploadErrorListAsync(request.ProcessId);
    }
}
