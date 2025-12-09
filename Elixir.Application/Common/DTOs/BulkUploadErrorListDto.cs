using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Common.DTOs
{
    public class BulkUploadErrorListDto
    {
        public int S_No { get; set; } //Serial number for the error record
        public string Field { get; set; } = String.Empty; //Name of the field where the error occurred
        public string Error { get; set; } = String.Empty; //Error message describing the issue

    }

}
