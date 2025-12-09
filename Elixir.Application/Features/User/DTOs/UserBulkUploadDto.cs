
namespace Elixir.Application.Features.User.DTOs;

public class UserBulkUploadDto
{
    public int RowId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public int EmailHash { get; set; } 
    public string TelephonePhoneNumber { get; set; } = String.Empty;
    public int TelephoneCodeId { get; set; } = 0; // This will be used to link to the TelephoneCodeMaster
    public string TelephoneCode { get; set; } = String.Empty;
    public string Designation { get; set; } = null!;
    public string Location { get; set; } = null!;
}
