using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sonneville.Investing.WebApi.AppStartup
{
    public class MvcStartup : StartupBase
    {
        public override void Configure(IApplicationBuilder applicationBuilder)
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }
    }
}