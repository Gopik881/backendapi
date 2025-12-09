namespace Elixir.Application.Features.Company.DTOs;

public class CompanyUserDto
{
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string RoleName { get; set; }
    public bool? Status { get; set; }
    public DateTime? CreatedOn { get; set; }

}
