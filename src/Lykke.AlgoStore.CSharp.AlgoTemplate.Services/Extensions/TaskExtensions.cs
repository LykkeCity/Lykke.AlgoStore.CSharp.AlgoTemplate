using Lykke.AlgoStore.CSharp.AlgoTemplate.Core;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Extensions
{
    public static class TaskExtensions
    {
        public static async Task<T> WithTimeout<T>(this Task<T> targetTask)
        {
            var timeout = Task.Delay(TimeSpan.FromSeconds(Constants.ServiceCallTimeoutSeconds));
            var taskWhichFinishedFirst = await Task.WhenAny(targetTask, timeout);
            if (taskWhichFinishedFirst != targetTask)
                throw new TaskCanceledException(targetTask);

            return await targetTask;
        }
    }
}
