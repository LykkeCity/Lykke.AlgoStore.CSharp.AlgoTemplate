using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class BaseAlgoInstance : BaseAlgoData
    {
        [Required]
        public string InstanceId { get; set; }
    }
}
