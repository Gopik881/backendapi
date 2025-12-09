using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Features.User.DTOs;
using Elixir.Application.Interfaces.Services;

namespace Elixir.Infrastructure.Services;

/// <summary>
/// Service to convert file data into DTOs for bulk upload operations.
/// It relies on a matrix representation of the data, where each row corresponds to a record
/// The order of columns in the matrix must match the expected order in the DTOs.
/// Ideally It should be metadata driven, but for simplicity, it is hardcoded here.
/// In future, we can enhance it to be more dynamic by using metadata or configuration files.
/// </summary>
public class FileDataConverterService : IFileDataConverterService
{
    public List<CountryBulkUploadDto> ConvertToCountryDto(List<List<string>> matrixData)
    {
        var countryDtos = new List<CountryBulkUploadDto>();
        if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "CountryName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "CountryShortName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "Description", StringComparison.OrdinalIgnoreCase)))
            return null;

        foreach (var row in matrixData.Skip(1)) //Skip Header
        {
            if (row.Count < 4) continue; // Ensure at least 4 values exist

            var dto = new CountryBulkUploadDto
            {
                RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                CountryName = row[1].Trim(),
                CountryShortName = row[2].Trim(),
                Description = row[3].Trim()
            };

            countryDtos.Add(dto);
        }
        return countryDtos;
    }

    public List<StateBulkUploadDto> ConvertToStateDto(List<List<string>> matrixData)
    {
        try
        {
            var stateDtos = new List<StateBulkUploadDto>();
            if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "CountryName *", StringComparison.OrdinalIgnoreCase)) &&
                (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "StateName *", StringComparison.OrdinalIgnoreCase)) &&
                (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "StateShortName *", StringComparison.OrdinalIgnoreCase)) &&
                (String.IsNullOrEmpty(matrixData[0][4]) || !String.Equals(matrixData[0][4], "Description", StringComparison.OrdinalIgnoreCase)))
                return null;
            foreach (var row in matrixData.Skip(1)) //Skip Header
            {
                if (row.Count < 5) continue; // Ensure at least 5 values exist

                var dto = new StateBulkUploadDto
                {

                    RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                    CountryName = row[1].Trim(),
                    StateName = row[2].Trim(),
                    StateShortName = row[3].Trim(),
                    Description = row[4].Trim()
                };

                stateDtos.Add(dto);
            }
            return stateDtos;
        }
        catch
        {
            throw new FormatException("The file format or uploaded template is invalid.");
        }
    }


    public List<StateBulkUploadDto> ConvertToStateDtoForCountry(List<List<string>> matrixData)
    {
        var stateDtos = new List<StateBulkUploadDto>();
        if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "CountryName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "StateName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "StateShortName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "Description", StringComparison.OrdinalIgnoreCase)))
            return null;
        foreach (var row in matrixData.Skip(1)) //Skip Header
        {
            if (row.Count < 4) continue; // Ensure at least 4 values exist

            var dto = new StateBulkUploadDto
            {

                RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                StateName = row[1].Trim(),
                StateShortName = row[2].Trim(),
                Description = row[3].Trim()
            };

            stateDtos.Add(dto);
        }
        return stateDtos;
    }

    public List<TelephoneCodeBulkUploadDto> ConvertToTelephoneCodeDto(List<List<string>> matrixData)
    {
        var telephoneCodeDtos = new List<TelephoneCodeBulkUploadDto>();
        if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "CountryName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "TelephoneCode *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "Description", StringComparison.OrdinalIgnoreCase)))
            return null;
        foreach (var row in matrixData.Skip(1)) //Skip Header
        {
            if (row.Count < 4) continue; // Ensure at least 4 values exist

            var dto = new TelephoneCodeBulkUploadDto
            {
                RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                CountryName = row[1].Trim(),
                TelephoneCode = row[2].Trim(),
                Description = row[3].Trim()
            };

            telephoneCodeDtos.Add(dto);
        }
        return telephoneCodeDtos;
    }

    public List<CurrencyBulkUploadDto> ConvertToCurrencyDto(List<List<string>> matrixData)
    {
        var currencyDtos = new List<CurrencyBulkUploadDto>();
        if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "CountryName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "CurrencyName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "CurrencyShortName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][4]) || !String.Equals(matrixData[0][4], "Description", StringComparison.OrdinalIgnoreCase)))
            return null;
        foreach (var row in matrixData.Skip(1)) //Skip Header
        {
            if (row.Count < 5) continue; // Ensure at least 5 values exist

            var dto = new CurrencyBulkUploadDto
            {
                RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                CountryName = row[1].Trim(),
                CurrencyName = row[2].Trim(),
                CurrencyShortName = row[3].Trim(),
                Description = row[4].Trim()
            };

            currencyDtos.Add(dto);
        }
        return currencyDtos;
    }

    public List<UserBulkUploadDto> ConvertToUserDto(List<List<string>> matrixData)
    {
        var userDtos = new List<UserBulkUploadDto>();
        if ((String.IsNullOrEmpty(matrixData[0][1]) || !String.Equals(matrixData[0][1], "FirstName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][2]) || !String.Equals(matrixData[0][2], "LastName *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][3]) || !String.Equals(matrixData[0][3], "Email *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][4]) || !String.Equals(matrixData[0][4], "PhoneCode *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][5]) || !String.Equals(matrixData[0][5], "PhoneNumber *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][6]) || !String.Equals(matrixData[0][6], "Location *", StringComparison.OrdinalIgnoreCase)) &&
            (String.IsNullOrEmpty(matrixData[0][7]) || !String.Equals(matrixData[0][7], "Designation *", StringComparison.OrdinalIgnoreCase)))
            return null;
        foreach (var row in matrixData.Skip(1)) //Skip Header
        {
            if (row.Count < 7) continue; // Ensure at least 7 values exist

            var dto = new UserBulkUploadDto
            {
                RowId = int.Parse(row[0].Trim()), // Assuming the first column is RowId
                FirstName = row[1].Trim(),
                LastName = row[2].Trim(),
                Email = row[3].Trim(),
                TelephoneCode = row[4].Trim(),
                TelephonePhoneNumber = row[5].Trim(),
                Location = row[6].Trim(),
                Designation = row[7].Trim(),
            };
            userDtos.Add(dto);
        }
        return userDtos;
    }

}
