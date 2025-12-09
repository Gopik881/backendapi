namespace Elixir.Domain.Entities;

public partial class WebQueryHorizontalCheckboxItem
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public int HorizontalId { get; set; }

    public string CheckboxItemName { get; set; } = null!;

    public bool? IsSelected { get; set; }

    public bool IsDeleted { get; set; }

}
