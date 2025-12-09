namespace Elixir.Domain.Entities;

public partial class ModuleMaster
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int ModuleId { get; set; }

    public string MasterName { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }
}
