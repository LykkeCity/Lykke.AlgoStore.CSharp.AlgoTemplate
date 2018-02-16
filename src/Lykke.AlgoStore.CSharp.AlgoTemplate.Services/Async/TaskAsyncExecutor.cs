using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async
{
    /// <summary>
    /// <see cref="IAsyncExecutor"/> implementation using <see cref="Task"/>
    /// to achieve asynchronous execution
    /// </summary>
    public class TaskAsyncExecutor : IAsyncExecutor
    {
        public Task ExecuteAsync(Action a) => Task.Run(a);

        public Task ExecuteAsync<T>(Func<T, Task> a, T param) => Task.Run(() => a(param));
    }
}
