using System;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// <see cref="IStatisticsService"/> implementation
    /// </summary>
    public class StatisticsService : IStatisticsService
    {
        private readonly IStatisticsRepository _statisticsRepository;
        private readonly IAlgoSettingsService _algoSettings;
        private readonly IUserLogRepository _userLogRepository;
        private string _instanceId;

        public StatisticsService(
            IStatisticsRepository statisticsRepository, 
            IAlgoSettingsService algoSettings,
            IUserLogRepository userLogRepository)
        {
            _statisticsRepository = statisticsRepository;
            _algoSettings = algoSettings;
            _userLogRepository = userLogRepository;
        }

        public IAlgoQuote GetQuote()
        {
            throw new NotImplementedException();
        }

        public void OnQuote(IAlgoQuote quote)
        {
            //REMARK: No need to save any statistics here (for now)
        }

        public void OnAction(bool isBuy, double volume, double price)
        {
            try
            {
                var data = new Statistics
                {
                    InstanceId = _instanceId,
                    Amount = volume,
                    IsBuy = isBuy,
                    Price = price
                };

                var summary = GetSummary();

                summary.TotalNumberOfTrades++;

                //TODO: Update other summary properties from external service(s) before save

                _statisticsRepository.CreateAsync(data, summary).Wait();
            }
            catch (Exception ex)
            {
                _userLogRepository.WriteAsync(_instanceId, ex).Wait();
                
                throw new Exception("Saving statistics failed", ex);
            }
        }

        public void OnAlgoStarted()
        {
            _instanceId = _algoSettings.GetInstanceId();

            try
            {
                var data = new Statistics
                {
                    InstanceId = _instanceId,
                    IsStarted = true
                };

                var summary = GetSummary();

                summary.TotalNumberOfStarts++;

                _statisticsRepository.CreateAsync(data, summary).Wait();
            }
            catch (Exception ex)
            {
                _userLogRepository.WriteAsync(_instanceId, ex).Wait();

                throw new Exception("Saving statistics failed", ex);
            }
        }

        public void OnAlgoStopped()
        {
            try
            {
                var data = new Statistics
                {
                    InstanceId = _instanceId,
                    IsStarted = false
                };

                _statisticsRepository.CreateAsync(data).Wait();
            }
            catch (Exception ex)
            {
                _userLogRepository.WriteAsync(_instanceId, ex).Wait();

                throw new Exception("Saving statistics failed", ex);
            }
        }

        public StatisticsSummary GetSummary()
        {
            try
            {
                return _statisticsRepository.GetSummaryAsync(_instanceId).Result;
            }
            catch (Exception ex)
            {
                _userLogRepository.WriteAsync(_instanceId, ex).Wait();

                throw new Exception("Get statistics summary failed", ex);
            }
        }
    }
}
