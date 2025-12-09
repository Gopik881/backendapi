namespace Elixir.Domain.Entities;

public partial class ModuleMappingHistory
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int CompanyId { get; set; }

    public int ModuleId { get; set; }

    public int? SubModuleId { get; set; }

    public bool? IsEnabled { get; set; }

    public bool? IsMandatory { get; set; }

    public int? Version { get; set; }

    public bool IsDeleted { get; set; }

}
