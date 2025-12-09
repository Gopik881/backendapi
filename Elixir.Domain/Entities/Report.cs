namespace Elixir.Domain.Entities;

public partial class Report
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? CreatedBy { get; set; }

    public int? UpdatedBy { get; set; }

    public string ReportName { get; set; } = null!;

    public int CategoryId { get; set; }

    public bool? IsSelected { get; set; }

    public bool IsDeleted { get; set; }

}
