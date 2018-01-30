using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
