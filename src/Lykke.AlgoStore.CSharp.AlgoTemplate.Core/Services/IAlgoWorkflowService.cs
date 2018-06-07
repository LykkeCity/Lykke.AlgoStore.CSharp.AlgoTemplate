using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    public interface IAlgoWorkflowService
    {
        Task StartAsync();
        Task StopAsync();
    }
}
