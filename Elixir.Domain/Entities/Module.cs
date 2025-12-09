namespace Elixir.Domain.Entities;

public partial class Module
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string ModuleName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string ModuleUrl { get; set; } = null!;

    public bool? IsEnabled { get; set; }

    public bool IsDeleted { get; set; }

}
