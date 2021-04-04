using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace ERP.Models.ViewModel.Roles
{
    public class SelectUserForRoleModel
    {
        public bool IsSelected { get; set; }

        public string Name { get; set; }

        public SelectUserForRoleModel()
        {

        }

        public SelectUserForRoleModel(IdentityRole role)
        {
            Name = role.Name;
        }
    }
}