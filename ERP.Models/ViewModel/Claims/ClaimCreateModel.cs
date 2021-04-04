using System.ComponentModel.DataAnnotations;

namespace ERP.Models.ViewModel.Claims
{
    public class ClaimCreateModel
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}