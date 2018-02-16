using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async
{
    /// <summary>
    /// Interface of executing asynchronously actions 
    /// </summary>
    public interface IAsyncExecutor
    {
        /// <summary>
        /// Executes the action provided and returns control to the caller
        /// </summary>
        /// <param name="a"><see cref="Action"/> to be executed</param>
        Task ExecuteAsync(Action a);

        /// <summary>
        /// Executes the action provided and returns control to the caller
        /// </summary>
        /// <typeparam name="T">The type of the action parameter</typeparam>
        /// <param name="a"><see cref="Action"/> to be executed</param>
        /// <param name="param">The action param</param>
        Task ExecuteAsync<T>(Func<T, Task> a, T param);
    }
}
