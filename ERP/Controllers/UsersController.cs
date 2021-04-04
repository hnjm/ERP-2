using ERP.DBAccess.Context;
using ERP.Helper.ExtensionMethod;
using ERP.Models.ViewModel.Users;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    public class UsersController : Controller
    {
        private readonly IdentityContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDataProtector _protector;

        public UsersController(IdentityContext context, UserManager<IdentityUser> userManager, IDataProtectionProvider provider, string purposeString)
        {
            _context = context;
            _userManager = userManager;
            _protector = provider.CreateProtector(purposeString);
        }

        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users
                .Select(x => new UsersViewModel(x, _protector))
                .ToArrayAsync().ConfigureAwait(false);

            return View(users);
        }

        [HttpGet("[controller]/Details/{id:length(176)}")]
        public async Task<IActionResult> Details(string id)
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
                    .ConfigureAwait(false),
                Claims = await _context.GetClaimsAsync(id, _protector)
                    .ConfigureAwait(false)
            };

            return View(userDetailsModel);
        }

        [HttpGet("[controller]/Create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("[controller]/Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var identityUser = new IdentityUser
            {
                UserName = model.Name,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password)
                .ConfigureAwait(false);

            if (result.Succeeded)
            {
                return RedirectToAction("GetUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpGet("[controller]/Edit/{id:length(176)}")]
        public async Task<IActionResult> Edit(string id)
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

            return View(new UsersEditViewModel(user, _protector));
        }

        [HttpPost("[controller]/Edit/{id:length(176)}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UsersEditViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
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

            user.UserName = model.Name;
            user.Email = model.Email;
            user.EmailConfirmed = model.IsVerified;

            var result = await _userManager.UpdateAsync(user)
                .ConfigureAwait(false);

            if (result.Succeeded)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user)
                    .ConfigureAwait(false);

                result = await _userManager.ResetPasswordAsync(user, token, model.Password)
                    .ConfigureAwait(false);
                if (result.Succeeded)
                {
                    return RedirectToAction("GetUsers");
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost("[controller]/Delete/{id:length(176)}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
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

            await _userManager.DeleteAsync(user).ConfigureAwait(false);

            return RedirectToAction("GetUsers");
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