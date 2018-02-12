using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain
{
    public class UserLog
    {
        public string InstanceId { get; set; }

        public string Message { get; set; }

        public DateTime Date { get; set; }
    }
}
