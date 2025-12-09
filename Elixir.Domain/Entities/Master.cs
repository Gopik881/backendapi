namespace Elixir.Domain.Entities;

public partial class Master
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string MasterName { get; set; } = null!;

    public string? MasterScreenUrl { get; set; }

    public bool IsDeleted { get; set; }
}
