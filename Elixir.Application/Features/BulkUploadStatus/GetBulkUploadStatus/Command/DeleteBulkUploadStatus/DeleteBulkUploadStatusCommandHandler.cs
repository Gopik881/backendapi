using Elixir.Application.Interfaces.Persistance;
using MediatR;

namespace Elixir.Application.Features.BulkUploadStatus.GetBulkUploadStatus.Command.DeleteBulkUploadStatus;

public record DeleteBulkUploadStatusCommand(Guid ProcessId) : IRequest<bool>;
public class DeleteBulkUploadStatusCommandHandler : IRequestHandler<DeleteBulkUploadStatusCommand, bool>
{
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public DeleteBulkUploadStatusCommandHandler(IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _bulkUploadErrorListRepository=bulkUploadErrorListRepository;
    }
    public async Task<bool> Handle(DeleteBulkUploadStatusCommand request, CancellationToken cancellationToken)
    {
        return await _bulkUploadErrorListRepository.DeleteBulkUploadErrorListAsync(request.ProcessId);
    }
}
