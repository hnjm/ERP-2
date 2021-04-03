using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Helper.ServiceProvider
{
    public class ValueProvider:IServiceProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            const string key = "this is my secret key";

            services.AddSingleton(key);
        }
    }
}