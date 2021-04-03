using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.DBAccess.Context;
using ERP.Models.ViewModel.Claims;
using ERP.Models.ViewModel.Roles;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

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
                .Select(x => x.ClaimType)
                .ToArrayAsync();

            var claimsView = new ClaimsViewModel[claims.Length];

            var count = 0;
            while (count != claimsView.Length)
            {
                claimsView[count] = new ClaimsViewModel
                    (await context.Claims.FirstAsync(x => x.Id == claims[count]), protector);
                count++;
            }

            return claimsView;
        }
    }
}
