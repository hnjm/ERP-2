using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace ERP.Models.ViewModel.Roles
{
    public class RolesViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public RolesViewModel()
        {

        }

        public RolesViewModel(IdentityRole role, IDataProtector protector)
        {
            Id = protector.Protect(role.Id);
            Name = role.Name;
        }
    }
}
