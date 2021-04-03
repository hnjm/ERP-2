using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

namespace ERP.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return default!;
        }
    }
}
