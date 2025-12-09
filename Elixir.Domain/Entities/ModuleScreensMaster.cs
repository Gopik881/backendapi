namespace Elixir.Domain.Entities;

public partial class ModuleScreensMaster
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string ModuleMasterName { get; set; } = null!;

    public int? SubModuleId { get; set; }

    public bool? IsModuleMasterType { get; set; }

    public bool IsDeleted { get; set; }

}
