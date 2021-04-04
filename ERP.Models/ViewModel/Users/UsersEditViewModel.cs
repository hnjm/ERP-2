using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Users
{
    public class UsersEditViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        public bool IsVerified { get; set; }

        public UsersEditViewModel()
        {

        }

        public UsersEditViewModel(IdentityUser user, IDataProtector protector)
        {
            Id = protector.Protect(user.Id);
            Name = user.UserName;
            Email = user.Email;
            IsVerified = user.EmailConfirmed;
        }
    }
}