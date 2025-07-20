using Microsoft.Extensions.Options;

namespace AwesomeCompany.Options
{
    public class ApplicationOptionsSetup : IConfigureOptions<ApplicationOptions>
    {
        private readonly IConfiguration config;

        public ApplicationOptionsSetup(IConfiguration config)
        {
            this.config = config;
        }

        public void Configure(ApplicationOptions options)
        {
            config.GetSection(nameof(ApplicationOptions))
                .Bind(options);
        }
    }
}
