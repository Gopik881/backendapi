namespace Elixir.Application.Features.User.DTOs;

public class UserProfileDto
{
    public int UserId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailId { get; set; }
    public string PhoneNo { get; set; }
    public int? TelePhoneCodeId { get; set; }
    public string? PhoneShortCutCode { get; set; }
    public string EmployeeLocation { get; set; }
    public string Designation { get; set; }
    public string? ProfilePicURL { get; set; }
    public bool? IsEnabled { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
}
