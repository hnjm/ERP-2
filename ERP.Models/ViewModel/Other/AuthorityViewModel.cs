using ERP.Models.ViewModel.Claims;
using ERP.Models.ViewModel.Roles;

namespace ERP.Models.ViewModel.Other
{
    public class AuthorityViewModel
    {
        public SelectUserForRoleModel[] Roles { get; set; }

        public SelectUserForClaimModel[] Claims { get; set; }
    }
}