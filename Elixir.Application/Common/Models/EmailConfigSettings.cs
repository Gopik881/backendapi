using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Application.Common.Models
{
    public class EmailConfigSettings
    {
        public string ConnectionString { get; set; }
        public string SenderAddress { get;set; }
        public string EncryptionKey { get; set; }
    }
}
