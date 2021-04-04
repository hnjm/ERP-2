using ERP.DBAccess.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Helper.ServiceProvider
{
    public class IdentityInstaller : IServiceProvider
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<IdentityUser, IdentityRole>(option =>
                {
                    option.User.RequireUniqueEmail = true;

                    option.User.AllowedUserNameCharacters += " ";

                    option.Password.RequireDigit = true;
                    option.Password.RequireLowercase = false;
                    option.Password.RequireNonAlphanumeric = false;
                    option.Password.RequireUppercase = false;
                    option.Password.RequiredLength = 6;
                })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();
        }
    }
}
