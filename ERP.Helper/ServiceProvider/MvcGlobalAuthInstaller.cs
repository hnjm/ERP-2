using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Helper.ServiceProvider
{
    public class MvcGlobalAuthInstaller : IServiceProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllersWithViews(option =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireRole("Admin")
                    .RequireClaim("View Data")
                    .Build();

                option.Filters.Add(new AuthorizeFilter(policy));

            });
        }
    }
}
