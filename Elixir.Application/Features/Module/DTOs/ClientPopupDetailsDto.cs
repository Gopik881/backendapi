namespace Elixir.Application.Features.Module.DTOs;

public class ClientPopupDetailsDto
{
    public int ClientId { get; set; }
    public string ClineName { get; set; }
    public int NoOfCompanies { get; set; }
    public int NoOfAccountManagers { get; set; }
    public string[] AccountManagers { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool? Status { get; set; }
}
