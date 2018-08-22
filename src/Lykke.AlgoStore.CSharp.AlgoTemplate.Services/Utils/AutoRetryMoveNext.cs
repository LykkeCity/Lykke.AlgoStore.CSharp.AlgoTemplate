using Microsoft.Rest;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public static class AutoRetryMoveNext
    {
        public static async Task<bool> MoveNextWithRetry<T>(this IEnumerator<T> enumerator, int retryCount,
            CancellationToken cancellationToken)
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
                    if (cancellationToken.IsCancellationRequested) return false;

                    currentRetryCount++;

                    if (currentRetryCount > retryCount)
                        throw;

                    await Task.Delay(10_000);
                }
            }
        }
    }
}
