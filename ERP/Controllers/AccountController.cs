using System.Threading.Tasks;
using ERP.Models.ViewModel.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(UserLoginView model)
        {
            if (!ModelState.IsValid)
                return View();

            var user = await _userManager.FindByNameAsync(model.UserName)
                .ConfigureAwait(false);

            if (user is null | await _userManager.CheckPasswordAsync(user, model.Password).ConfigureAwait(false))
            {
                ModelState.AddModelError("", "Invalid Name or Password");
                return View();
            }

            await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true)
                .ConfigureAwait(false);

            return LocalRedirect("~");
        }
        
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}