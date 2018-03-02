using System;
using System.Collections.Generic;
using System.Text;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class FeeSettings
    {
        public TargetClientIdFeeSettings TargetClientId { get; set; }

        public class TargetClientIdFeeSettings
        {
            public string Hft { get; set; }
        }
    }
}
