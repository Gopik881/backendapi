using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Application.Features.Telephone.Commands.BulkInsertTelephoneCodes;

public record BulkInsertTelephoneCodesCommand(List<TelephoneCodeBulkUploadDto> TelephoneCodes, string FileName) : IRequest<BulkUploadStatusDto>;
public class BulkInsertTelephoneCodesCommandHandler : IRequestHandler<BulkInsertTelephoneCodesCommand, BulkUploadStatusDto>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly ITelephoneCodeMasterRepository _telephoneCodeMasterRepository;
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public BulkInsertTelephoneCodesCommandHandler(ICountryMasterRepository countryMasterRepository, ITelephoneCodeMasterRepository telephoneCodeMasterRepository, IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _telephoneCodeMasterRepository = telephoneCodeMasterRepository;
        _bulkUploadErrorListRepository = bulkUploadErrorListRepository;
    }
    public async Task<BulkUploadStatusDto> Handle(BulkInsertTelephoneCodesCommand request, CancellationToken cancellationToken)
    {
        //var fileSizeLimit = await _countryMasterRepository.GetBulkUploadFileSizeLimitMbAsync();
        //if (fileSizeLimit.HasValue && request.TelephoneCodes.Count > fileSizeLimit.Value * 1024 * 1024) //Check if the number of countries exceeds the file size limit in MB
        //{
        //    throw new ValidationException($"The number of TelephoneCodes exceeds the file size limit of {fileSizeLimit.Value} MB.");
        //}
        bool IsBulkInsertSuccessful = false; //Initialize result to false
        Guid NewProcessId = Guid.NewGuid();
        BulkUploadStatusDto bulkUploadStatusDto = new BulkUploadStatusDto() { ProcessId = NewProcessId, FileName = request.FileName, ProcessedAt = DateTime.UtcNow };
        List<BulkUploadErrorList> errorLists = new List<BulkUploadErrorList>();
        List<TelephoneCodeBulkUploadDto> tcToInsert = new List<TelephoneCodeBulkUploadDto>();

        // Get existing telephone codes from the database to a HashSet for faster lookup
        var existingTelephoneCodesRecords = await _telephoneCodeMasterRepository.GetAllTelephoneCodesAsync();
        var existingTelephoneCodes = new HashSet<Tuple<int, string>>(existingTelephoneCodesRecords.Select(s => Tuple.Create(s.CountryId, s.TelephoneCode)));
        // Get existing countries from the database to a dictionary for faster lookup Names will be the Key and IDs will be the Value
        var existingCountriesDictionary = (await _countryMasterRepository.GetAllCountriesAsync()).ToDictionary(c => c.CountryName.ToLower(), c => c.CountryId);

        //Update the CountryId in each telephoneCodesBulkUploadDto based on the existing countries
        foreach (var record in request.TelephoneCodes)
        {
            //Check if the country exists in the database
            if (existingCountriesDictionary.TryGetValue(record.CountryName.ToLower(), out int countryId))
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
        foreach (var record in request.TelephoneCodes)
        {
            // Skip records that don't have a valid CountryId mapped (they were already reported as country errors)
            if (record.CountryId <= 0)
            {
                continue;
            }

            // Normalize telephone code: trim and ensure it starts with '+'
            var rawCode = record.TelephoneCode?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(rawCode))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Telephone Code", ErrorMessage = "Telephone Code is required" });
                continue;
            }
            if (!rawCode.StartsWith("+"))
            {
                rawCode = "+" + rawCode;
            }
            record.TelephoneCode = rawCode;

            // Validate that only digits exist after '+'
            var digitsPart = rawCode.Length > 1 ? rawCode.Substring(1) : string.Empty;
            if (string.IsNullOrEmpty(digitsPart) || !digitsPart.All(char.IsDigit))
            {
                errorLists.Add(new BulkUploadErrorList()
                {
                    ProcessId = NewProcessId,
                    RowId = record.RowId,
                    ErrorField = "Telephone Code",
                    ErrorMessage = $"Invalid Telephone Code '{record.TelephoneCode}'. Only digits are allowed after '+'."
                });
                continue;
            }

            // Check for duplicates against existing telephone codes
            if (existingTelephoneCodes.Contains(Tuple.Create(record.CountryId, record.TelephoneCode)))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = record.RowId, ErrorField = "Telephone Code", ErrorMessage = $"Duplicate Entry for {record.TelephoneCode}" });
            }
            else
            {
                tcToInsert.Add(record);
            }
        }
        if (tcToInsert?.Any() == true) //If there are telephone codes to insert, call the repository method to insert them
        {
            IsBulkInsertSuccessful = await _telephoneCodeMasterRepository.BulkInsertTelephoneCodesAsync(tcToInsert);
        }
        if (errorLists.Count > 0)
        {
            await _bulkUploadErrorListRepository.BulkInsertBulkUploadErrorListAsync(errorLists);
        }

        bulkUploadStatusDto.IsSuccess = false;
        if (IsBulkInsertSuccessful && errorLists.Count() == 0) bulkUploadStatusDto.IsSuccess = true;
        if (IsBulkInsertSuccessful && errorLists.Count() > 0) bulkUploadStatusDto.IsPartialSuccess = true;

        return bulkUploadStatusDto;

    }
}
