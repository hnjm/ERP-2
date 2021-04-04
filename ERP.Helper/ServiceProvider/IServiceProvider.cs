using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Helper.ServiceProvider
{
    public interface IServiceProvider
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
