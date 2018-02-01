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
        public void Execute(Action a) => Task.Run(a);

        public void ExecuteAsync<T>(Action<T> a, T param) => Task.Run(() => a(param));
    }
}
