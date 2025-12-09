using Elixir.Application.Common.Constants;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using MediatR;

namespace Elixir.Application.Features.Currency.Commands.DeleteCurrency;

public record DeleteCurrencyCommand(int CurrencyId) : IRequest<bool>;
public class DeleteCurrencyCommandHandler : IRequestHandler<DeleteCurrencyCommand, bool>
{
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    private readonly IEntityReferenceService _entityReferenceService;

    public DeleteCurrencyCommandHandler(
        ICurrencyMasterRepository currencyMasterRepository,
        IEntityReferenceService entityReferenceService)
    {
        _currencyMasterRepository = currencyMasterRepository;
        _entityReferenceService = entityReferenceService;
    }

    public async Task<bool> Handle(DeleteCurrencyCommand request, CancellationToken cancellationToken)
    {
        if (await _entityReferenceService.HasActiveReferencesAsync(nameof(request.CurrencyId), request.CurrencyId))
            throw new Exception(AppConstants.ErrorCodes.CURRENCY_MASTER_DELETE_FAILED);

        return await _currencyMasterRepository.DeleteAsync(request.CurrencyId);
    }
}

