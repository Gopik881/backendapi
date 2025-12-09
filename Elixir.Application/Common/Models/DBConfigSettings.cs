
namespace Elixir.Application.Common.Models
{
    public class DBConfigSettings
    {
        public string ConnectionStringTemplate { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }
        public string Password { get; set; }
        public string ElasticPoolName { get; set; } = string.Empty;
        public string TenantDbTemplate { get; set; } = string.Empty;

    }
}
