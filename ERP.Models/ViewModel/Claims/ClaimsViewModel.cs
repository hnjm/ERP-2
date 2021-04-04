using ERP.Models.DBModels.Claim;
using Microsoft.AspNetCore.DataProtection;
using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Claims
{
    public class ClaimsViewModel
    {
        public string Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }

        public ClaimsViewModel()
        {

        }

        public ClaimsViewModel(ClaimDbModel claim, IDataProtector protector)
        {
            Id = protector.Protect(claim.Id);
            Type = claim.Type;
            Value = claim.Value;
        }
    }
}
