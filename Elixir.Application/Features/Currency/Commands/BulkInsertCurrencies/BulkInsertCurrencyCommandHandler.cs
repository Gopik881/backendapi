using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Application.Features.Currency.Commands.BulkInsertCurrencies;

public  record BulkInsertCurrencyCommand(List<CurrencyBulkUploadDto> Currencies,string FileName) : IRequest<BulkUploadStatusDto>;
public class BulkInsertCurrencyCommandHandler : IRequestHandler<BulkInsertCurrencyCommand, BulkUploadStatusDto>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly ICurrencyMasterRepository _currencyMasterRepository;
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public BulkInsertCurrencyCommandHandler(ICountryMasterRepository countryMasterRepository, ICurrencyMasterRepository currencyMasterRepository, IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _currencyMasterRepository = currencyMasterRepository;
        _bulkUploadErrorListRepository = bulkUploadErrorListRepository;
    }
    // Pseudocode plan:
    // 1. Get all existing currencies and countries.
    // 2. For each record in request.Currencies:
    //    a. Set CountryId from CountryName.
    //    b. If CountryName not found, add error.
    // 3. For each record:
    //    a. If a currency for the country already exists, add error.
    //    b. If the currency short name exists for another currency, add error.
    //    c. If the currency name exists for another currency, add error.
    //    d. If all checks pass, add to insert list.
    // 4. Bulk insert valid currencies.
    // 5. Bulk insert errors.
    // 6. Set BulkUploadStatusDto accordingly.

    public async Task<BulkUploadStatusDto> Handle(BulkInsertCurrencyCommand request, CancellationToken cancellationToken)
    {
        bool IsBulkInsertSuccessful = false;
        Guid NewProcessId = Guid.NewGuid();
        BulkUploadStatusDto bulkUploadStatusDto = new BulkUploadStatusDto() { ProcessId = NewProcessId, FileName = request.FileName, ProcessedAt = DateTime.UtcNow };
        List<BulkUploadErrorList> errorLists = new List<BulkUploadErrorList>();
        List<CurrencyBulkUploadDto> currenciesToInsert = new List<CurrencyBulkUploadDto>();

        var existingCurrencies = (await _currencyMasterRepository.GetAllCurrenciesAsync()).ToList();
        var existingCountriesDictionary = (await _countryMasterRepository.GetAllCountriesAsync())
            .ToDictionary(c => c.CountryName.ToLower(), c => c.CountryId);

        // Set CountryId for each record
        foreach (var record in request.Currencies)
        {
            if (existingCountriesDictionary.TryGetValue(record.CountryName.ToLower(), out int countryId))
            {
                record.CountryId = countryId;
            }
            else
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Country Name", ErrorMessage = $"Country '{record.CountryName}' not found" });
                continue;
            }
        }

        // Prepare sets for validation
        var existingCurrencyNames = new HashSet<string>(existingCurrencies.Select(c => c.CurrencyName.ToLower()));
        var existingShortNames = new HashSet<string>(existingCurrencies.Select(c => c.CurrencyShortName.ToLower()));
        var existingCountryIds = new HashSet<int>(existingCurrencies.Select(c => c.CountryId));

        var processedCountryIds = new HashSet<int>();
        var processedCurrencyNames = new HashSet<string>(existingCurrencyNames);
        var processedShortNames = new HashSet<string>(existingShortNames);

        foreach (var record in request.Currencies)
        {
            // Only one currency per country allowed
            if (processedCountryIds.Contains(record.CountryId) || existingCountryIds.Contains(record.CountryId))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Country", ErrorMessage = $"Currency already exists for country '{record.CountryName}'" });
                continue;
            }

            // Currency name must be unique globally
            if (processedCurrencyNames.Contains(record.CurrencyName.ToLower()))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Currency Name", ErrorMessage = $"Currency name '{record.CurrencyName}' already exists for another currency" });
                continue;
            }

            // Currency short name must be unique globally
            if (processedShortNames.Contains(record.CurrencyShortName.ToLower()))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Currency Short Name", ErrorMessage = $"Currency short name '{record.CurrencyShortName}' already exists for another currency" });
                continue;
            }

            currenciesToInsert.Add(record);
            processedCountryIds.Add(record.CountryId);
            processedCurrencyNames.Add(record.CurrencyName.ToLower());
            processedShortNames.Add(record.CurrencyShortName.ToLower());
        }

        if (currenciesToInsert.Any())
        {
            IsBulkInsertSuccessful = await _currencyMasterRepository.BulkInsertCurrenciesAsync(currenciesToInsert);
        }
        if (errorLists.Count > 0)
        {
            await _bulkUploadErrorListRepository.BulkInsertBulkUploadErrorListAsync(errorLists);
        }

        bulkUploadStatusDto.IsSuccess = false;
        if (IsBulkInsertSuccessful && errorLists.Count == 0) bulkUploadStatusDto.IsSuccess = true;
        if (IsBulkInsertSuccessful && errorLists.Count > 0) bulkUploadStatusDto.IsPartialSuccess = true;

        return bulkUploadStatusDto;
    }
}
