using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Roles
{
    public class RolesViewModel
    {
        [Required]
        [StringLength(maximumLength: 176, MinimumLength = 176)]
        public string Id { get; set; }

        [Required]
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
