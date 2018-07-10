using System.Threading;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services
{
    /// <summary>
    /// Provides various functions for monitoring the algo instance operations
    /// </summary>
    public interface IMonitoringService
    {
        /// <summary>
        /// Starts a cancelable timeout after which the instance will be stopped
        /// </summary>
        /// <returns>A <see cref="CancellationTokenSource"/> which can be cancelled to stop the timeout</returns>
        CancellationTokenSource StartAlgoEvent();
    }
}
