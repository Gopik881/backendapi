namespace Elixir.Domain.Entities;

public partial class SubModule
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string SubModuleName { get; set; } = null!;

    public int? ModuleId { get; set; }

    public bool? IsEnabled { get; set; }

    public int? SubModuleParentId { get; set; }

    public bool IsDeleted { get; set; }

}
