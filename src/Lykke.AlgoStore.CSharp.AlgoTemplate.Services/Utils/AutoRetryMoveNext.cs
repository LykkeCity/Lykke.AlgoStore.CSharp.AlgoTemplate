using Microsoft.Rest;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public static class AutoRetryMoveNext
    {
        public static async Task<bool> MoveNextWithRetry<T>(this IEnumerator<T> enumerator, int retryCount)
        {
            var currentRetryCount = retryCount;

            while (true)
            {
                try
                {
                    return enumerator.MoveNext();
                }
                catch (HttpOperationException)
                {
                    currentRetryCount++;

                    if (currentRetryCount > retryCount)
                        throw;

                    await Task.Delay(10_000);
                }
            }
        }
    }
}
