using ERP.Models.ViewModel.Roles;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [Route("[controller]/[action]")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDataProtector _protector;

        public RolesController(RoleManager<IdentityRole> roleManager, IDataProtectionProvider provider, string purposeString)
        {
            _roleManager = roleManager;
            _protector = provider.CreateProtector(purposeString);
        }

        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles
                .Select(x => new RolesViewModel(x, _protector));

            return View(roles);
        }

        [HttpGet("{id:length(176)}")]
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

            var role = await _roleManager.Roles
                .Where(x => x.Id == id)
                .Select(x => new RolesViewModel(x, _protector))
                .FirstOrDefaultAsync();

            if (role is null)
                return NotFound();

            return View(role);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string name)
        {
            if (name is null)
                return View();

            var role = new IdentityRole
            {
                Name = name
            };

            var result = await _roleManager.CreateAsync(role)
                .ConfigureAwait(false);

            if (result.Succeeded)
                return RedirectToAction("GetRoles");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View();
        }

        [HttpGet("{id:length(176)}")]
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

            var role = await _roleManager.Roles
                .Where(x => x.Id == id)
                .Select(x => new RolesViewModel(x, _protector))
                .FirstOrDefaultAsync();

            if (role is null)
                return NotFound();

            return View(role);
        }

        [HttpPost("{id:length(176)}")]
        public async Task<IActionResult> Edit(string id, RolesViewModel model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                id = _protector.Unprotect(id);
            }
            catch
            {
                return NotFound();
            }

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role is null)
                return NotFound();

            role.Name = model.Name;

            var result = await _roleManager.CreateAsync(role)
                .ConfigureAwait(false);

            if (result.Succeeded)
                return RedirectToAction("GetRoles");

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        [HttpPost("{id:length(176)}")]
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

            var role = await _roleManager.Roles
                .FirstOrDefaultAsync(x => x.Id == id);

            if (role is null)
                return NotFound();

            await _roleManager.DeleteAsync(role)
                .ConfigureAwait(false);

            return RedirectToAction("GetRoles");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _roleManager.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
