namespace Elixir.Domain.Entities;

public partial class ClientAccess
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool? EnableWebQuery { get; set; }

    public bool? EnableReportAccess { get; set; }

    public int? ClientUserLimit { get; set; }

    public int? ClientId { get; set; }

    public bool IsDeleted { get; set; }
    
}
