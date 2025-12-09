using Elixir.Application.Common.DTOs;
using Elixir.Application.Interfaces.Persistance;
using Elixir.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Drawing;
using System.Text.RegularExpressions;

namespace Elixir.Infrastructure.Services;


/// <summary>
/// Service for handling file operations related to bulk uploads.
/// The service provides methods to check file format compatibility, if supported,
/// The worksheet column order is important and should match the expected order in the DTOs.
/// Ideally It should be metadata driven, but for simplicity, it is hardcoded here.
/// In future, we can enhance it to be more dynamic by using metadata or configuration files.
/// </summary>
public class FileHandlingService : IFileHandlingService
{
    private readonly ISystemPoliciesRepository _systemPoliciesRepository;

    public FileHandlingService(ISystemPoliciesRepository systemPoliciesRepository)
    {
        _systemPoliciesRepository = systemPoliciesRepository;
    }

    public bool IsFileFormatSupportedForBulkUpload(IFormFile file)
    {
        var systmePolicy = _systemPoliciesRepository.GetDefaultSystemPolicyAsync().Result;

        if (file == null || file.Length == 0 || file.Length > (systmePolicy.FileSizeLimitMb * 1024 * 1024))
        {
            return false;
        }
        //Only supporting .xlsx as the Country Data should be added as a DropDown for them to select.  
        return Path.GetExtension(file.FileName).ToLowerInvariant() == ".xlsx";
    }

    public List<List<string>> ProcessXlsx(IFormFile file)
    {
        var matrixData = new List<List<string>>();

        using var package = new ExcelPackage(file.OpenReadStream());
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");
        var worksheet = package.Workbook.Worksheets[0];
        if (worksheet == null || worksheet.Dimension == null) return null;

        //Define the below structure for the matrixData  
        // matrixData.Add(new List<string>() { "Row", "Column1", "Column2", ... });  
        for (int row = 1; row <= worksheet.Dimension.End.Row; row++)
        {
            bool isRowEmpty = worksheet.Cells[row, 1, row, worksheet.Dimension.End.Column].All(cell => string.IsNullOrEmpty(cell.Text));
            if (isRowEmpty) break;

            var rowData = new List<string>() { row.ToString() };
            for (int col = 1; col <= worksheet.Dimension.End.Column; col++)
            {
                rowData.Add(worksheet.Cells[row, col].Text);
            }
            matrixData.Add(rowData);
        }

        return matrixData;
    }

    // Public validator: returns true when the uploaded file's first row headers exactly match expectedHeaders (positional).
    // Caller should display "invalid template" to user when this returns false.
    public bool ValidateTemplateHeaders(IFormFile file, List<string> expectedHeaders)
    {
        if (file == null || file.Length == 0 || expectedHeaders == null || expectedHeaders.Count == 0)
            return false;

        using var package = new ExcelPackage(file.OpenReadStream());
        ExcelPackage.License.SetNonCommercialOrganization("TMI");
        var worksheet = package.Workbook.Worksheets.FirstOrDefault();
        if (worksheet == null || worksheet.Dimension == null) return false;

        int headerColumns = worksheet.Dimension.End.Column;
        // If column counts differ -> invalid (covers extra columns case)
        if (headerColumns != expectedHeaders.Count) return false;

        for (int col = 1; col <= headerColumns; col++)
        {
            var actual = NormalizeHeaderText(worksheet.Cells[1, col].Text);
            var expected = NormalizeHeaderText(expectedHeaders[col - 1]);
            if (!string.Equals(actual, expected, StringComparison.OrdinalIgnoreCase))
                return false;
        }

        return true;
    }

    // Normalizes header text: trims, removes '*' characters, compresses whitespace.
    private static string NormalizeHeaderText(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;
        // Remove asterisks and any trailing/leading whitespace, normalize inner spaces
        var withoutAsterisk = input.Replace("*", "");
        var trimmed = withoutAsterisk.Trim();
        // Collapse multiple spaces into single space
        var normalized = Regex.Replace(trimmed, @"\s+", " ");
        return normalized;
    }

    public Task<byte[]> GetCountriesBulkUploadTemplateAsync()
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Countries");
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "CountryName *";
        worksheet.Cells[1, 2].Value = "CountryShortName *";
        worksheet.Cells[1, 3].Value = "Description";

        // Style only the asterisks in red for mandatory headers
        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("CountryName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("CountryShortName ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }

    public Task<byte[]> GetStatesBulkUploadTemplateAsync(List<string> countries)
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("States");
        // Setup Hidden worksheet for countries and give it as a DropDown in the States worksheet
        var worksheet_hidden = package.Workbook.Worksheets.Add("Countries");
        int lastRow = worksheet_hidden.Dimension != null ? worksheet_hidden.Dimension.End.Row : 0;
        foreach (var country in countries)
        {
            // Add each country to the hidden worksheet
            worksheet_hidden.Cells[lastRow + 1, 1].Value = country;
            lastRow++;
        }
        var range = worksheet.Cells[2, 1, 1000, 1]; // Assuming max rows = 1000 as per the previous code structure
        var validation = worksheet.DataValidations.AddListValidation(range.Address);
        validation.Formula.ExcelFormula = $"'{worksheet_hidden.Name}'!$A$1:$A${countries.Count}";

        validation.ShowErrorMessage = true;
        validation.ErrorTitle = "Invalid Selection";
        validation.Error = "Please select a value from the dropdown list only.";
        validation.AllowBlank = false;
        validation.ShowInputMessage = true;


        worksheet_hidden.Hidden = eWorkSheetHidden.Hidden;
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "CountryName *";
        worksheet.Cells[1, 2].Value = "StateName *";
        worksheet.Cells[1, 3].Value = "StateShortName *";
        worksheet.Cells[1, 4].Value = "Description";

        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("CountryName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("StateName ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 3].RichText.Clear();
        worksheet.Cells[1, 3].RichText.Add("StateShortName ").Color = Color.Black;
        worksheet.Cells[1, 3].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 4].Value = "Description";
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }

    public Task<byte[]> GetStatesBulkUploadTemplateForCountryAsync()
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("States");
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "StateName *";
        worksheet.Cells[1, 2].Value = "StateShortName *";
        worksheet.Cells[1, 3].Value = "Description";

        // Define the headers for the template
        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("StateName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("StateShortName ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;


        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }


    public Task<byte[]> GetTelephoneCodeBulkUploadTemplateAsync(List<string> countries)
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("TelephoneCode");
        //Setup Hidden worksheet for countries and give it as a DropDown in the States worksheet
        var worksheet_hidden = package.Workbook.Worksheets.Add("Countries");
        int lastRow = worksheet_hidden.Dimension != null ? worksheet_hidden.Dimension.End.Row : 0;
        foreach (var country in countries)
        {
            // Add each country to the hidden worksheet
            worksheet_hidden.Cells[lastRow + 1, 1].Value = country;
            lastRow++;
        }
        var range = worksheet.Cells[2, 1, 1000, 1]; // Assuming max rows = 1000 as per the previous code strcuture
        var validation = worksheet.DataValidations.AddListValidation(range.Address);
        validation.Formula.ExcelFormula = $"'{worksheet_hidden.Name}'!$A$1:$A${countries.Count}";

        validation.ShowErrorMessage = true;
        validation.ErrorTitle = "Invalid Selection";
        validation.Error = "Please select a value from the dropdown list only.";
        validation.AllowBlank = false;
        validation.ShowInputMessage = true;

        worksheet_hidden.Hidden = eWorkSheetHidden.Hidden;
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "CountryName *";
        worksheet.Cells[1, 2].Value = "TelephoneCode *";
        worksheet.Cells[1, 3].Value = "Description";

        // Define the headers for the template
        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("CountryName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("TelephoneCode ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;

        // Set TelephoneCode column to Text format and add a default value with '+'
        var telephoneCodeRange = worksheet.Cells[2, 2, 1000, 2];
        telephoneCodeRange.Style.Numberformat.Format = "@"; // Set as text        

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }
    public Task<byte[]> GetCurrenciesBulkUploadTemplateAsync(List<string> countries)
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Currencies");
        //Setup Hidden worksheet for countries and give it as a DropDown in the States worksheet
        var worksheet_hidden = package.Workbook.Worksheets.Add("Countries");
        int lastRow = worksheet_hidden.Dimension != null ? worksheet_hidden.Dimension.End.Row : 0;
        foreach (var country in countries)
        {
            // Add each country to the hidden worksheet
            worksheet_hidden.Cells[lastRow + 1, 1].Value = country;
            lastRow++;
        }
        var range = worksheet.Cells[2, 1, 1000, 1]; // Assuming max rows = 1000 as per the previous code strcuture
        var validation = worksheet.DataValidations.AddListValidation(range.Address);
        validation.Formula.ExcelFormula = $"'{worksheet_hidden.Name}'!$A$1:$A${countries.Count}";

        validation.ShowErrorMessage = true;
        validation.ErrorTitle = "Invalid Selection";
        validation.Error = "Please select a value from the dropdown list only.";
        validation.AllowBlank = false;
        validation.ShowInputMessage = true;

        worksheet_hidden.Hidden = eWorkSheetHidden.Hidden;
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "CountryName *";
        worksheet.Cells[1, 2].Value = "CurrencyName *";
        worksheet.Cells[1, 3].Value = "CurrencyShortName *";
        worksheet.Cells[1, 4].Value = "Description";


        // Define the headers for the template
        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("CountryName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("CurrencyName ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 3].RichText.Clear();
        worksheet.Cells[1, 3].RichText.Add("CurrencyShortName ").Color = Color.Black;
        worksheet.Cells[1, 3].RichText.Add("*").Color = Color.Red;



        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }


    public Task<byte[]> GetUsersBulkUploadTemplateAsync(List<string> telephoneCodes)
    {
        // New way for EPPlus 8+
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Users");
        //Setup Hidden worksheet for countries and give it as a DropDown in the States worksheet
        var worksheet_hidden = package.Workbook.Worksheets.Add("TelephoneCodes");
        int lastRow = worksheet_hidden.Dimension != null ? worksheet_hidden.Dimension.End.Row : 0;
        foreach (var telephoneCode in telephoneCodes)
        {
            // Add each country to the hidden worksheet
            worksheet_hidden.Cells[lastRow + 1, 1].Value = telephoneCode;
            lastRow++;
        }
        // Define the range for the dropdown validation for the telephone codes
        var range = worksheet.Cells[2, 4, 1000, 4]; // Assuming max rows = 1000 as per the previous code strcuture
        var validation = worksheet.DataValidations.AddListValidation(range.Address);
        validation.Formula.ExcelFormula = $"'{worksheet_hidden.Name}'!$A$1:$A${telephoneCodes.Count}";

        validation.ShowErrorMessage = true;
        validation.ErrorTitle = "Invalid Selection";
        validation.Error = "Please select a value from the dropdown list only.";
        validation.AllowBlank = false;
        validation.ShowInputMessage = true;

        worksheet_hidden.Hidden = eWorkSheetHidden.Hidden;
        // Define the headers for the template
        worksheet.Cells[1, 1].Value = "FirstName *";
        worksheet.Cells[1, 2].Value = "LastName *";
        worksheet.Cells[1, 3].Value = "Email *";
        worksheet.Cells[1, 4].Value = "PhoneCode *";
        worksheet.Cells[1, 5].Value = "PhoneNumber *";
        worksheet.Cells[1, 6].Value = "Location *";
        worksheet.Cells[1, 7].Value = "Designation *";

        // Define the headers for the template
        worksheet.Cells[1, 1].RichText.Clear();
        worksheet.Cells[1, 1].RichText.Add("FirstName ").Color = Color.Black;
        worksheet.Cells[1, 1].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 2].RichText.Clear();
        worksheet.Cells[1, 2].RichText.Add("LastName ").Color = Color.Black;
        worksheet.Cells[1, 2].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 3].RichText.Clear();
        worksheet.Cells[1, 3].RichText.Add("Email ").Color = Color.Black;
        worksheet.Cells[1, 3].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 4].RichText.Clear();
        worksheet.Cells[1, 4].RichText.Add("PhoneCode ").Color = Color.Black;
        worksheet.Cells[1, 4].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 5].RichText.Clear();
        worksheet.Cells[1, 5].RichText.Add("PhoneNumber ").Color = Color.Black;
        worksheet.Cells[1, 5].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 6].RichText.Clear();
        worksheet.Cells[1, 6].RichText.Add("Location ").Color = Color.Black;
        worksheet.Cells[1, 6].RichText.Add("*").Color = Color.Red;

        worksheet.Cells[1, 7].RichText.Clear();
        worksheet.Cells[1, 7].RichText.Add("Designation ").Color = Color.Black;
        worksheet.Cells[1, 7].RichText.Add("*").Color = Color.Red;


        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
        return Task.FromResult(package.GetAsByteArray());
    }


    public Task<byte[]> GetBulkUploadStatusDownloadAsync(List<BulkUploadErrorListDto> lists)
    {
        ExcelPackage.License.SetNonCommercialOrganization("TMI");

        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Errors");
        worksheet.Cells[1, 1].Value = "S.No.";
        worksheet.Cells[1, 2].Value = "Field";
        worksheet.Cells[1, 3].Value = "Error";
        int lastRow = worksheet.Dimension != null ? worksheet.Dimension.End.Row : 0;
        foreach (var item in lists)
        {
            worksheet.Cells[lastRow + 1, 1].Value = item.S_No.ToString();
            worksheet.Cells[lastRow + 1, 2].Value = item.Field;
            worksheet.Cells[lastRow + 1, 3].Value = item.Error;
            lastRow++;
        }
        return Task.FromResult(package.GetAsByteArray());
    }

}
