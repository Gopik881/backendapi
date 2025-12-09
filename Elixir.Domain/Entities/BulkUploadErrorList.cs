namespace Elixir.Domain.Entities
{
    public class BulkUploadErrorList
    {
        public long Id { get; set; } 
        public Guid ProcessId { get; set; }
        public int RowId { get; set; } //Row ID in the source data where the error occurred
        public string ErrorField { get; set; } = String.Empty;
        public string ErrorMessage { get; set; } = String.Empty; //Error message describing the issue
    }
}
