using System.ComponentModel.DataAnnotations;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models
{
    public class Statistics
    {
        [Required]
        public string InstanceId { get; set; }

        public string Id { get; set; }

        public bool? IsBuy { get; set; }

        public double? Price { get; set; }

        public double? Amount { get; set; }

        public bool? IsStarted { get; set; }
    }
}
