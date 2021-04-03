using System.ComponentModel.DataAnnotations;

namespace ERP.Models.DBModels.Claim
{
    public class ClaimDbModel
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string Type { get; set; }

        [Required]
        public string Value { get; set; }
    }
}
