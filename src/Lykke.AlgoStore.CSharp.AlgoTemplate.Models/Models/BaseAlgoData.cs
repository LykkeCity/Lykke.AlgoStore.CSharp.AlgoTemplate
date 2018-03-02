using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class BaseAlgoData : BaseValidatableData
    {
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string AlgoId { get; set; }
    }
}
