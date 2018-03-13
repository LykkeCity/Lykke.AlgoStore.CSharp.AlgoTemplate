using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Service providing trading capabilities
    /// </summary>
    public interface ITradingService
    {
        void Initialize();
        Task<ResponseModel<double>> BuyStraight(double volume);
        Task<ResponseModel<double>> SellStraight(double volume);
    }
}
