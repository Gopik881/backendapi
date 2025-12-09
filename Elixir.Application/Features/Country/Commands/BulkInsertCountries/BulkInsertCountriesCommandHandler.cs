using Elixir.Application.Common.DTOs;
using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Domain.Entities;
using FluentValidation;
using MediatR;

namespace Elixir.Application.Features.Country.Commands.BulkInsert;

public record BulkInsertCountriesCommand(List<CountryBulkUploadDto> Countries,string FileName) : IRequest<BulkUploadStatusDto>;
public class BulkInsertCountriesCommandHandler : IRequestHandler<BulkInsertCountriesCommand, BulkUploadStatusDto>
{
    private readonly ICountryMasterRepository _countryMasterRepository;
    private readonly IBulkUploadErrorListRepository _bulkUploadErrorListRepository;
    public BulkInsertCountriesCommandHandler(ICountryMasterRepository countryMasterRepository, IBulkUploadErrorListRepository bulkUploadErrorListRepository)
    {
        _countryMasterRepository = countryMasterRepository;
        _bulkUploadErrorListRepository=bulkUploadErrorListRepository;
    }
    public async Task<BulkUploadStatusDto> Handle(BulkInsertCountriesCommand request, CancellationToken cancellationToken)
    {
        
        bool IsBulkInsertSuccessful = false; //Initialize result to false
        Guid NewProcessId = Guid.NewGuid();
        BulkUploadStatusDto bulkUploadStatusDto = new BulkUploadStatusDto() { ProcessId= NewProcessId,FileName=request.FileName,ProcessedAt=DateTime.UtcNow };
        List<BulkUploadErrorList> errorLists = new List<BulkUploadErrorList>();
        List<CountryBulkUploadDto> countriesToInsert = new List<CountryBulkUploadDto>();
        var existingCountries = await _countryMasterRepository.GetAllCountriesAsync();
        var existingCountryNames = new HashSet<string>(existingCountries.Select(c => c.CountryName.ToLower()));
        var existingCountryShortNames = new HashSet<string>(existingCountries.Select(c => c.CountryShortName.ToLower()));

        bool isValidRecord = true;
        foreach (var country in request.Countries)
        {
            if (existingCountryNames.Contains(country.CountryName.ToLower()))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId= NewProcessId, RowId = country.RowId, ErrorField = "Country",  ErrorMessage = $"Duplicate Entry for {country.CountryName}" });
                isValidRecord = false;
            }
            if (existingCountryShortNames.Contains(country.CountryShortName.ToLower()))
            {
                errorLists.Add(new BulkUploadErrorList() { ProcessId = NewProcessId, RowId = country.RowId, ErrorField = "Short Name", ErrorMessage = $"Duplicate Entry for {country.CountryShortName}" });
                isValidRecord = false;
            }
            if(isValidRecord) countriesToInsert.Add(country); //If the country does not exist, add it to the list of countries to insert
            isValidRecord = true; //Reset
        }
        if (countriesToInsert?.Any()==true) //If there are countries to insert, call the repository method to insert them
        {
            IsBulkInsertSuccessful = await _countryMasterRepository.BulkInsertCountriesAsync(countriesToInsert);
        }
        //Insert Errors to Error Table
        if (errorLists.Count>0)
        {
            
            await _bulkUploadErrorListRepository.BulkInsertBulkUploadErrorListAsync(errorLists);
        }

        bulkUploadStatusDto.IsSuccess = false;
        if (IsBulkInsertSuccessful && errorLists.Count() == 0)  bulkUploadStatusDto.IsSuccess=true;
        if (IsBulkInsertSuccessful && errorLists.Count() > 0) bulkUploadStatusDto.IsPartialSuccess=true;
        
        return bulkUploadStatusDto;
    }
}


 