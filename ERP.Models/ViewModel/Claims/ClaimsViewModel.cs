using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Models.DBModels.Claim;
using Microsoft.AspNetCore.DataProtection;

namespace ERP.Models.ViewModel.Claims
{
    public class ClaimsViewModel
    {
        public string Id { get; set; }

        public string Type { get; set; }

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
