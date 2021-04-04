using ERP.Models.DBModels.Claim;
using Microsoft.AspNetCore.DataProtection;

namespace ERP.Models.ViewModel.Claims
{
    public class SelectUserForClaimModel
    {
        public string Type { get; set; }

        public string Value { get; set; }

        public bool IsSelected { get; set; }

        public SelectUserForClaimModel()
        {

        }

        public SelectUserForClaimModel(ClaimDbModel claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }
    }
}