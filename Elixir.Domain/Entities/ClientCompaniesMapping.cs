namespace Elixir.Domain.Entities;

public partial class ClientCompaniesMapping
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int ClientId { get; set; }

    public int CompanyId { get; set; }

    public bool IsDeleted { get; set; }

}
