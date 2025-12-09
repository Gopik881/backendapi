using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Common.DTOs
{
    public class BulkUploadStatusDto
    {
        public string FileName { get; set; } = String.Empty; //Name of the file where the error occurred
        public Guid ProcessId { get; set; } //Unique identifier for the bulk upload process
        public DateTime ProcessedAt { get; set; } //Timestamp when the bulk upload was processed
        public bool IsSuccess { get; set;} = false;
        public bool IsPartialSuccess { get; set; } = false;
        public string AdditionalMessage{ get; set; }
        
    }
}
