namespace Elixir.Domain.Entities;

public partial class Category
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public bool IsEnabled { get; set; }

    public string CategoryName { get; set; } = null!;

    public bool IsDeleted { get; set; }
    
}
