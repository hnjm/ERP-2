using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Users
{
    public class UserLoginView
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}