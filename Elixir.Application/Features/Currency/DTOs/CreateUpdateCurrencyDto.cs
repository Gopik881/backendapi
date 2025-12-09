using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Features.Currency.DTOs
{
    public class CreateUpdateCurrencyDto
    {
        // Default initialized to 0 (keeps nullable type for compatibility with create operations)
        public int? CurrencyId { get; set; } = 0;
        public int CountryId { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyShortName { get; set; }
        public string Description { get; set; }
    }
}
