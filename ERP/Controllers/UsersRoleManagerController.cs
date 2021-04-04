using ERP.DBAccess.Context;
using ERP.Helper.ExtensionMethod;
using ERP.Models.ViewModel.Roles;
using ERP.Models.ViewModel.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [Route("[controller]/[action]")]
    public class UsersRoleManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataProtector _protector;

        public UsersRoleManagerController(UserManager<IdentityUser> userManager, IdentityContext context,
            RoleManager<IdentityRole> roleManager, IDataProtectionProvider provider, string purposeString)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _protector = provider.CreateProtector(purposeString);
        }

        [HttpGet("{id:length(176)}")]
        public async Task<IActionResult> ManageUserRole(string id)
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

            var userDetailsModel = new UsersDetailsViewModel(user, _protector)
            {
                Claims = await _context.GetClaimsAsync(id, _protector)
                    .ConfigureAwait(false)
            };

            var selectUserForRole = await _context.Roles
                .Select(x => new SelectUserForRoleModel(x, _protector))
                .ToArrayAsync().ConfigureAwait(false);

            foreach (var model in selectUserForRole)
            {
                model.IsSelected = await _userManager.IsInRoleAsync(user, model.Name)
                    .ConfigureAwait(false);
            }

            ViewBag.User = userDetailsModel;

            return View(selectUserForRole);
        }

        [HttpPost("{id:length(176)}")]
        public async Task<IActionResult> ManageUserRole(string id, SelectUserForRoleModel[] model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageUserRole");
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

            foreach (var selectUserForRoleModel in model)
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

            return RedirectToAction("Details", "Users", new { id = _protector.Protect(user.Id) });
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
