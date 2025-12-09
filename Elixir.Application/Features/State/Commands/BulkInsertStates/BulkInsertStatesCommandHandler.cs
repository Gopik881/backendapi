using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Application.Features.State.Commands.BulkInsertStates;
public record BulkInsertStatesCommand(List<StateBulkUploadDto> States,int? countryId, string FileName) : IRequest<BulkUploadStatusDto>;

public class BulkInsertStatesCommandHandler : IRequestHandler<BulkInsertStatesCommand, BulkUploadStatusDto>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IStateMasterRepository _stateMasterRepository;
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public BulkInsertStatesCommandHandler(ICountryMasterRepository countryMasterRepository, IStateMasterRepository stateMasterRepository, IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _stateMasterRepository = stateMasterRepository;
        _bulkUploadErrorListRepository = bulkUploadErrorListRepository;

    }
    public async Task<BulkUploadStatusDto> Handle(BulkInsertStatesCommand request, CancellationToken cancellationToken)
    {
        bool IsBulkInsertSuccessful = false; //Initialize result to false
        Guid NewProcessId = Guid.NewGuid();
        BulkUploadStatusDto bulkUploadStatusDto = new BulkUploadStatusDto() { ProcessId = NewProcessId, FileName = request.FileName, ProcessedAt = DateTime.UtcNow };
        List<BulkUploadErrorList> errorLists = new List<BulkUploadErrorList>();
        List<StateBulkUploadDto> statesToInsert = new List<StateBulkUploadDto>();

        // Load existing states and build lookups for name and short name per country
        var existingStates = await _stateMasterRepository.GetAllStatesAsync();
        var existingNameSet = new HashSet<(int countryId, string stateName)>(
            existingStates
                .Select(s => (s.CountryId, (s.StateName ?? string.Empty).Trim().ToLowerInvariant()))
                .Where(t => !string.IsNullOrEmpty(t.Item2))
        );
        var existingShortSet = new HashSet<(int countryId, string stateShortName)>(
            existingStates
                .Select(s => (s.CountryId, (s.StateShortName ?? string.Empty).Trim().ToLowerInvariant()))
                .Where(t => !string.IsNullOrEmpty(t.Item2))
        );

        // Resolve CountryId for incoming records
        if (request.countryId.HasValue && request.countryId.Value > 0) //If a countryId is provided in the request, we will use it to set the CountryId for each state
        {
            //Check if the country exists in the database first, if not we will return a status message indicating the country does not exist
            if (!await _countryMasterRepository.ExistsAsync(request.countryId.Value))
            {
                bulkUploadStatusDto.AdditionalMessage = "Failed, No such Country found.";
                bulkUploadStatusDto.IsSuccess = false;
                return bulkUploadStatusDto;
            }
            else
            {
                //If the country exists, set the CountryId for each StateBulkUploadDto in the request
                request.States.ForEach(record => record.CountryId = request.countryId.Value);
            }
        }
        else
        {
            //CountryId is not provided, we will try to find the CountryId based on the CountryName in each StateBulkUploadDto
            // Get existing countries from the database to a dictionary for faster lookup Names will be the Key and IDs will be the Value
            var existingCountriesDictionary = (await _countryMasterRepository.GetAllCountriesAsync())
                .ToDictionary(c => (c.CountryName ?? string.Empty).Trim().ToLowerInvariant(), c => c.CountryId);

            //Update the CountryId in each StateBulkUploadDto based on the existing countries
            foreach (var record in request.States)
            {
                if (string.IsNullOrWhiteSpace(record.CountryName))
                {
                    errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Country Name", ErrorMessage = $"Country name is empty for row {record.RowId}" });
                    continue;
                }

                //Check if the country exists in the database
                var countryKey = record.CountryName.Trim().ToLowerInvariant();
                if (existingCountriesDictionary.TryGetValue(countryKey, out int countryId))
                {
                    //If it exists, set the CountryId in the record
                    record.CountryId = countryId;
                }
                else
                {
                    //If it does not exist, add a status message and skip this record
                    errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Country Name", ErrorMessage = $"Country '{record.CountryName}' not found" });
                    continue;
                }
            }
        }

        // Validate uniqueness on StateName and StateShortName separately (both against DB and within the uploaded file)
        var seenNameInFile = new HashSet<(int countryId, string stateName)>();
        var seenShortInFile = new HashSet<(int countryId, string stateShortName)>();

        foreach (var record in request.States)
        {
            // If CountryId is not set or invalid, it was already reported; skip
            if (record.CountryId <= 0)
                continue;

            var stateNameNorm = (record.StateName ?? string.Empty).Trim().ToLowerInvariant();
            var stateShortNameNorm = (record.StateShortName ?? string.Empty).Trim().ToLowerInvariant();

            // If either state name or short name are empty, treat as error
            if (string.IsNullOrEmpty(stateNameNorm))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "State", ErrorMessage = "State name is required" });
                continue;
            }
            if (string.IsNullOrEmpty(stateShortNameNorm))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "State Short Name", ErrorMessage = "State short name is required" });
                continue;
            }

            var nameKey = (record.CountryId, stateNameNorm);
            var shortKey = (record.CountryId, stateShortNameNorm);

            var perRowErrors = new List<BulkUploadErrorList>();

            // Check against existing DB by StateName
            if (existingNameSet.Contains(nameKey))
            {
                perRowErrors.Add(new BulkUploadErrorList()
                {
                    ProcessId = NewProcessId,
                    RowId = record.RowId,
                    ErrorField = "State",
                    ErrorMessage = $"Duplicate entry for State '{record.StateName}' in the same country."
                });
            }

            // Check against existing DB by StateShortName
            if (existingShortSet.Contains(shortKey))
            {
                perRowErrors.Add(new BulkUploadErrorList()
                {
                    ProcessId = NewProcessId,
                    RowId = record.RowId,
                    ErrorField = "State Short Name",
                    ErrorMessage = $"Duplicate entry for State Short Name '{record.StateShortName}' in the same country."
                });
            }

            // Check duplicates within the uploaded file by StateName
            if (seenNameInFile.Contains(nameKey))
            {
                perRowErrors.Add(new BulkUploadErrorList()
                {
                    ProcessId = NewProcessId,
                    RowId = record.RowId,
                    ErrorField = "State",
                    ErrorMessage = $"Duplicate entry within file for State '{record.StateName}' in the same country."
                });
            }

            // Check duplicates within the uploaded file by StateShortName
            if (seenShortInFile.Contains(shortKey))
            {
                perRowErrors.Add(new BulkUploadErrorList()
                {
                    ProcessId = NewProcessId,
                    RowId = record.RowId,
                    ErrorField = "State Short Name",
                    ErrorMessage = $"Duplicate entry within file for State Short Name '{record.StateShortName}' in the same country."
                });
            }

            if (perRowErrors.Count > 0)
            {
                errorLists.AddRange(perRowErrors);
                continue;
            }

            // Passed validations - queue for insert and mark seen
            seenNameInFile.Add(nameKey);
            seenShortInFile.Add(shortKey);
            statesToInsert.Add(record);
        }

        if (statesToInsert?.Any() == true) //If there are states to insert, call the repository method to insert them
        {
            IsBulkInsertSuccessful = await _stateMasterRepository.BulkInsertStatesAsync(statesToInsert);
        }
        if (errorLists.Count > 0)
        {
            await _bulkUploadErrorListRepository.BulkInsertBulkUploadErrorListAsync(errorLists);
        }

        // Determine final status
        bulkUploadStatusDto.IsSuccess = false;
        bulkUploadStatusDto.IsPartialSuccess = false;
        if (IsBulkInsertSuccessful && errorLists.Count == 0) bulkUploadStatusDto.IsSuccess = true;
        if (IsBulkInsertSuccessful && errorLists.Count > 0) bulkUploadStatusDto.IsPartialSuccess = true;

        return bulkUploadStatusDto;
    }
}
