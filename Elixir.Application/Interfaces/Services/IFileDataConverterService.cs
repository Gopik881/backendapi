using Elixir.Application.Features.Country.DTOs;
using Elixir.Application.Features.Currency.DTOs;
using Elixir.Application.Features.State.DTOs;
using Elixir.Application.Features.Telephone.DTOs;
using Elixir.Application.Features.User.DTOs;

namespace Elixir.Application.Interfaces.Services;

public interface IFileDataConverterService
{
    List<CountryBulkUploadDto> ConvertToCountryDto(List<List<string>> matrixData);
    List<CurrencyBulkUploadDto> ConvertToCurrencyDto(List<List<string>> matrixData);
    List<StateBulkUploadDto> ConvertToStateDto(List<List<string>> matrixData);
    List<StateBulkUploadDto> ConvertToStateDtoForCountry(List<List<string>> matrixData);
    List<TelephoneCodeBulkUploadDto> ConvertToTelephoneCodeDto(List<List<string>> matrixData);
    List<UserBulkUploadDto> ConvertToUserDto(List<List<string>> matrixData);
}
