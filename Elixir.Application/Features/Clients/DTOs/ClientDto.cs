namespace Elixir.Application.Features.Clients.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string ClientCode { get; set; } = string.Empty;
        public int NoOfCompanies { get; set; }
        public int NoOfUsers { get; set; }
        public bool? Status { get; set; }
        public DateTime? CreatedOn { get; set; }


    }
}
