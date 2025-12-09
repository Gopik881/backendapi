namespace Elixir.Domain.Entities;

public partial class SubModuleScreen
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int SubModuleId { get; set; }

    public int SubModuleMasterId { get; set; }

    public string ScreenName { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsDeleted { get; set; }
}
