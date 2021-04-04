using ERP.DBAccess.Context;
using ERP.Helper.ExtensionMethod;
using ERP.Models.ViewModel.Other;
using ERP.Models.ViewModel.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using ERP.Models.ViewModel.Roles;
using Microsoft.EntityFrameworkCore;
using ERP.Models.ViewModel.Claims;
using System.Security.Claims;

namespace ERP.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthorityController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataProtector _protector;

        public AuthorityController(UserManager<IdentityUser> userManager, IdentityContext context,
            RoleManager<IdentityRole> roleManager, IDataProtectionProvider provider, string purposeString)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _protector = provider.CreateProtector(purposeString);
        }

        [HttpGet("{id:length(176)}")]
        public async Task<IActionResult> ManageAuthority(string id)
        {
            try
            {
                id = _protector.Unprotect(id);
            }
            catch
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id)
                .ConfigureAwait(false);

            if (user is null)
                return NotFound();

            ViewBag.User = new UsersDetailsViewModel(user, _protector);

            var authorityModel = new AuthorityViewModel
            {
                Roles = await GetSelectUserForRoleModelsAsync(user)
                    .ConfigureAwait(false),
                Claims = await GetSelectUserForClaimModelsAsync(user)
                    .ConfigureAwait(false)
            };

            return View(authorityModel);
        }

        [HttpPost("{id:length(176)}")]
        public async Task<IActionResult> ManageAuthority(string id, AuthorityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageAuthority");
            }

            try
            {
                id = _protector.Unprotect(id);
            }
            catch
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id)
                .ConfigureAwait(false);

            if (user is null)
                return NotFound();

            foreach (var selectUserForRoleModel in model.Roles)
            {
                var exist = await _roleManager.RoleExistsAsync(selectUserForRoleModel.Name);

                if (!exist)
                    return BadRequest();

                var isInRole = await _userManager.IsInRoleAsync(user, selectUserForRoleModel.Name)
                    .ConfigureAwait(false);

                switch (isInRole)
                {
                    case true when !selectUserForRoleModel.IsSelected:
                        await _userManager.RemoveFromRoleAsync(user, selectUserForRoleModel.Name)
                            .ConfigureAwait(false);
                        break;
                    case false when selectUserForRoleModel.IsSelected:
                        await _userManager.AddToRoleAsync(user, selectUserForRoleModel.Name)
                            .ConfigureAwait(false);
                        break;
                }
            }

            foreach (var claimModel in model.Claims)
            {
                var exist = await _context.Claims
                    .AnyAsync(x => x.Type == claimModel.Type && x.Value == claimModel.Value)
                    .ConfigureAwait(false);

                if (!exist)
                    return BadRequest();

                var hasClaim = await _context.HasClaimAsync(claimModel.Type, claimModel.Value, user.Id)
                    .ConfigureAwait(false);

                var claim = new Claim(claimModel.Type, claimModel.Value);

                switch (hasClaim)
                {
                    case true when !claimModel.IsSelected:
                        await _userManager.RemoveClaimAsync(user, claim)
                            .ConfigureAwait(false);
                        break;
                    case false when claimModel.IsSelected:
                        await _userManager.AddClaimAsync(user, claim)
                            .ConfigureAwait(false);
                        break;
                }
            }

            return RedirectToAction("Details", "Users",
            new { id = _protector.Protect(id) });
        }

        private async Task<SelectUserForClaimModel[]> GetSelectUserForClaimModelsAsync(IdentityUser user)
        {
            var selectUserForClaimModels = await _context.Claims
                .Select(x => new SelectUserForClaimModel(x))
                .ToArrayAsync().ConfigureAwait(false);

            foreach (var model in selectUserForClaimModels)
            {
                model.IsSelected = await _context.HasClaimAsync(model.Type, model.Value, user.Id)
                    .ConfigureAwait(false);
            }

            return selectUserForClaimModels;
        }

        private async Task<SelectUserForRoleModel[]> GetSelectUserForRoleModelsAsync(IdentityUser user)
        {
            var selectUserForRoleModels = await _context.Roles
                .Select(x => new SelectUserForRoleModel(x))
                .ToArrayAsync().ConfigureAwait(false);

            foreach (var model in selectUserForRoleModels)
            {
                model.IsSelected = await _userManager.IsInRoleAsync(user, model.Name)
                    .ConfigureAwait(false);
            }

            return selectUserForRoleModels;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
                _roleManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
