using System.Net;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings
{
    public class MatchingEngineAdapterSettings
    {
        public string IpEndpoint { get; set; }
        public ushort Port { get; set; }

        public IPAddress GetClientIpAddress()
        {
            if (IPAddress.TryParse(IpEndpoint, out var ipAddress))
                return ipAddress;

            var addresses = Dns.GetHostAddressesAsync(IpEndpoint).Result;
            return addresses[0];
        }
    }
}
