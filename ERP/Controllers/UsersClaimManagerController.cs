using ERP.DBAccess.Context;
using ERP.Helper.ExtensionMethod;
using ERP.Models.ViewModel.Claims;
using ERP.Models.ViewModel.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [Route("[controller]/[action]")]
    public class UsersClaimManagerController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IdentityContext _context;
        private readonly IDataProtector _protector;

        public UsersClaimManagerController(UserManager<IdentityUser> userManager, IdentityContext context, IDataProtectionProvider provider, string purposeString)
        {
            _userManager = userManager;
            _context = context;
            _protector = provider.CreateProtector(purposeString);
        }

        [HttpGet("{id:length(176)}")]
        public async Task<IActionResult> ManageUserClaim(string id)
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
                Roles = await _context.GetRolesAsync(id, _protector)
                .ConfigureAwait(false)
            };

            var selectUserForClaimModels = await _context.Claims
                .Select(x => new SelectUserForClaimModel(x, _protector))
                .ToArrayAsync().ConfigureAwait(false);

            foreach (var model in selectUserForClaimModels)
            {
                model.IsSelected = await _context.HasClaimAsync(model.Type, model.Value, user.Id)
                    .ConfigureAwait(false);
            }

            ViewBag.User = userDetailsModel;

            return View(selectUserForClaimModels);
        }

        [HttpPost("{id:length(176)}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserClaim(string id, SelectUserForClaimModel[] model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("ManageUserClaim");
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

            foreach (var claimModel in model)
            {
                var exist = await _context.Claims.AnyAsync(x => x.Type == claimModel.Type
                        && x.Value == claimModel.Value).ConfigureAwait(false);

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

            return RedirectToAction("Details", "Users", new { id = _protector.Protect(user.Id) });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
                _userManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
