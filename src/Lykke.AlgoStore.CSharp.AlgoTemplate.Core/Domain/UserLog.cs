using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class UserLog
    {
        [Required]
        public string AlgoId { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
