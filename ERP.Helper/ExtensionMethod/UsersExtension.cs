using ERP.DBAccess.Context;
using ERP.Models.ViewModel.Claims;
using ERP.Models.ViewModel.Roles;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Helper.ExtensionMethod
{
    public static class UsersExtension
    {
        public static async Task<RolesViewModel[]> GetRolesAsync(this IdentityContext context, string id, IDataProtector protector)
        {
            var roleIds = await context.UserRoles
                .Where(x => x.UserId == id)
                .Select(x => x.RoleId)
                .ToArrayAsync();

            var rolesView = new RolesViewModel[roleIds.Length];

            var count = 0;
            while (count != rolesView.Length)
            {
                rolesView[count] = new RolesViewModel
                (await context.Roles.FirstAsync(x => x.Id == roleIds[count]), protector);
                count++;
            }

            return rolesView;
        }

        public static async Task<ClaimsViewModel[]> GetClaimsAsync(this IdentityContext context, string id, IDataProtector protector)
        {
            var claims = await context.UserClaims
                .Where(x => x.UserId == id)
                .Select(x => new { x.ClaimType, x.ClaimValue })
                .ToArrayAsync();

            var claimsView = new ClaimsViewModel[claims.Length];

            var count = 0;
            while (count != claimsView.Length)
            {
                var claim = await context.Claims
                    .FirstAsync(x => x.Type == claims[count].ClaimType && x.Value == claims[count].ClaimValue)
                    .ConfigureAwait(false);

                claimsView[count] = new ClaimsViewModel(claim, protector);
                count++;
            }

            return claimsView;
        }

        public static Task<bool> HasClaimAsync(this IdentityContext context, string name, string value, string id)
        {
            return context.UserClaims.AnyAsync(x => x.ClaimType == name
                                               && x.ClaimValue == value && x.UserId == id);
        }
    }
}
