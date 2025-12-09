using Elixir.Application.Common.DTOs;
using Microsoft.AspNetCore.Http;
namespace Elixir.Application.Interfaces.Services;

public interface IFileHandlingService
{
    bool IsFileFormatSupportedForBulkUpload(IFormFile file);

    public bool ValidateTemplateHeaders(IFormFile file, List<string> expectedHeaders);
    List<List<string>> ProcessXlsx(IFormFile file);
    Task<byte[]> GetCountriesBulkUploadTemplateAsync();
    Task<byte[]> GetStatesBulkUploadTemplateForCountryAsync();
    Task<byte[]> GetStatesBulkUploadTemplateAsync(List<string> countries);
    Task<byte[]> GetTelephoneCodeBulkUploadTemplateAsync(List<string> countries);
    Task<byte[]> GetCurrenciesBulkUploadTemplateAsync(List<string> countries);
    Task<byte[]> GetUsersBulkUploadTemplateAsync(List<string> telephoneCodes);
    Task<byte[]> GetBulkUploadStatusDownloadAsync(List<BulkUploadErrorListDto> lists);

}
