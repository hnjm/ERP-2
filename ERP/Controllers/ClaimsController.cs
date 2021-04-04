using ERP.DBAccess.Context;
using ERP.Models.DBModels.Claim;
using ERP.Models.ViewModel.Claims;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Controllers
{
    [Route("[controller]/[action]")]
    public class ClaimsController : Controller
    {
        private readonly IdentityContext _context;
        private readonly IDataProtector _protector;

        public ClaimsController(IdentityContext context, IDataProtectionProvider provider, string purposeString)
        {
            _context = context;
            _protector = provider.CreateProtector(purposeString);
        }

        public async Task<IActionResult> GetClaims()
        {
            var claims = await _context.Claims
                .Select(x => new ClaimsViewModel(x, _protector))
                .ToArrayAsync()
                .ConfigureAwait(false);

            return View(claims);
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

            var claim = await _context.Claims.FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);

            if (claim is null)
                return NotFound();

            claim.Id = _protector.Protect(claim.Id);
            return View(claim);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClaimCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _context.Claims.AddAsync(new ClaimDbModel
            {
                Id = Guid.NewGuid().ToString(),
                Type = model.Type,
                Value = model.Value
            });

            await _context.SaveChangesAsync();

            return RedirectToAction("GetClaims");
        }

        [HttpGet("{id:length(176)}")]
        public async Task<IActionResult> Edit(string id)
        {
            return await Details(id).ConfigureAwait(false);
        }

        [HttpPost("{id:length(176)}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ClaimDbModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (id != model.Id)
                return BadRequest();
            try
            {
                model.Id = _protector.Unprotect(id);
            }
            catch
            {
                return NotFound();
            }

            _context.Claims.Update(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetClaims");
        }

        [HttpPost("{id:length(176)}")]
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

            var claim = await _context.Claims.FirstOrDefaultAsync(x => x.Id == id)
                .ConfigureAwait(false);

            if (claim is null)
                return NotFound();

            _context.Claims.Remove(claim);
            await _context.SaveChangesAsync();

            return RedirectToAction("GetClaims");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
