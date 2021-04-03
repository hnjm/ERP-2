using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace ERP.Models.ViewModel.Users
{
    public class UsersViewModel
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public UsersViewModel()
        {

        }

        public UsersViewModel(IdentityUser user, IDataProtector protector)
        {
            Id = protector.Protect(user.Id);
            UserName = user.UserName;
            Email = user.Email;
        }
    }
}
