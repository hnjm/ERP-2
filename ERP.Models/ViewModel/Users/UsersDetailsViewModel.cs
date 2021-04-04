using ERP.Models.ViewModel.Claims;
using ERP.Models.ViewModel.Roles;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Users
{
    public class UsersDetailsViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Name")]
        public string UserName { get; set; }

        public string Email { get; set; }

        public bool IsVerified { get; set; }

        public RolesViewModel[] Roles { get; set; }

        public ClaimsViewModel[] Claims { get; set; }

        public UsersDetailsViewModel(IdentityUser user, IDataProtector protector)
        {
            Id = protector.Protect(user.Id);
            UserName = user.UserName;
            Email = user.Email;
            IsVerified = user.EmailConfirmed;
        }
    }
}
