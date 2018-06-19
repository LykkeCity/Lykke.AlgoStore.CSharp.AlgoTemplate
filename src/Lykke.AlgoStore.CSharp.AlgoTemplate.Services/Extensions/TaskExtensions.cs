using Lykke.AlgoStore.CSharp.AlgoTemplate.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> WithCancellation<T>(this Task<T> task)
        {
            var token = new CancellationTokenSource(TimeSpan.FromSeconds(Constants.ServiceCallTimoutSeconds)).Token;
            return await task.ContinueWith(t => t.GetAwaiter().GetResult(), token);
        }
    }
}
