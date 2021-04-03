using ERP.DBAccess.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Helper.ServiceProvider
{
    public class ContextInstaller:IServiceProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContextPool<IdentityContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("ERP"));
            });
        }
    }
}
