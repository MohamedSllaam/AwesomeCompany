using Microsoft.Extensions.Options;

namespace AwesomeCompany.Options
{
    public class DataBaseOptionsSetup : IConfigureOptions<DataBaseOptions>
    {
        private const string ConfiurationSectionName = "DatabaseOptions";
        private readonly IConfiguration _configuration;

        public DataBaseOptionsSetup(IConfiguration configuration)
        {
           _configuration = configuration;
        }

        public void Configure(DataBaseOptions options)
        {
           var connectionString = _configuration.GetConnectionString("Database");

            options.Co = connectionString;    
        }
    }
}
