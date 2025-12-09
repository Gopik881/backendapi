namespace Elixir.Application.Features.Notification.DTOs
{
    public class CompanyAdminDetailsDto
    {
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string ClientName { get; set; }
    }
}
