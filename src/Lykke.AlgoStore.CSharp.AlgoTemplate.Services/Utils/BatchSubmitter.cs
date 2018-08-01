using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils
{
    public sealed class BatchSubmitter<T> : IDisposable
    {
        private readonly BatchBlock<T> _batchBlock;
        private readonly ActionBlock<T[]> _actionBlock;

        private readonly TimeSpan _maxBatchLifetime;
        private readonly Timer _timer;

        private bool _isDisposing;

        public BatchSubmitter(
            TimeSpan maxBatchLifetime,
            int batchSizeThreshold,
            Func<T[], Task> batchAction)
        {
            if (batchAction == null)
                throw new ArgumentNullException(nameof(batchAction));

            if (maxBatchLifetime < TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(maxBatchLifetime), maxBatchLifetime,
                    "Should be positive time span");
            }

            if (batchSizeThreshold < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(batchSizeThreshold), batchSizeThreshold,
                    "Should be positive number");
            }

            _maxBatchLifetime = maxBatchLifetime;

            _batchBlock = new BatchBlock<T>(batchSizeThreshold);

            _actionBlock = new ActionBlock<T[]>(batchAction,
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 2 });

            _batchBlock.LinkTo(_actionBlock);

            _timer = new Timer((state) => _batchBlock.TriggerBatch(), null, maxBatchLifetime, maxBatchLifetime);
        }

        public void Enqueue(T item)
        {
            CheckDisposed();

            _batchBlock.Post(item);
        }

        public void Enqueue(IEnumerable<T> items)
        {
            CheckDisposed();

            if (items == null)
                throw new ArgumentNullException(nameof(items));

            foreach (var item in items)
                _batchBlock.Post(item);
        }

        public void Dispose()
        {
            _isDisposing = true;

            _timer.Dispose();

            _batchBlock.Complete();
            _batchBlock.Completion.ConfigureAwait(false).GetAwaiter().GetResult();

            _actionBlock.Complete();
            _actionBlock.Completion.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private void CheckDisposed()
        {
            if (_isDisposing)
                throw new ObjectDisposedException("BatchSubmitter");
        }
    }
}
