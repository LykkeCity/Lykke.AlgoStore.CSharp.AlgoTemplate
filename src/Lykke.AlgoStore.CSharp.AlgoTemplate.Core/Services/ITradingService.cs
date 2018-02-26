using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing trading capabilities
    /// </summary>
    public interface ITradingService
    {
        void Initialise();
        Task<double> BuyStraight(double volume);
        Task<double> SellStraight(double volume);
    }
}
