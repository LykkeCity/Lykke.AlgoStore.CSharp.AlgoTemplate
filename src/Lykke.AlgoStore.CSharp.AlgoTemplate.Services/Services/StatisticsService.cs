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
        private string _instanceId;
        private AlgoInstanceType _instanceType;

        public StatisticsService(IStatisticsRepository statisticsRepository, IAlgoSettingsService algoSettings)
        {
            _statisticsRepository = statisticsRepository;
            _algoSettings = algoSettings;
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
            var data = new Statistics
            {
                InstanceId = _instanceId,
                Amount = volume,
                IsBuy = isBuy,
                Price = price,
                InstanceType = _instanceType
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public void OnAlgoStarted(double initialWalletBalance, double assetOneBalance, double assetTwoBalance)
        {
            _instanceId = _algoSettings.GetInstanceId();
            _instanceType = _algoSettings.GetInstanceType();

            if (!_statisticsRepository.SummaryExistsAsync(_instanceId).Result)
            {
                var summaryData = new StatisticsSummary
                {
                    InstanceId = _instanceId,
                    TotalNumberOfTrades = 0,
                    InstanceType = _instanceType,
                    TotalNumberOfStarts = 0,
                    AssetOneBalance = assetOneBalance,
                    AssetTwoBalance = assetTwoBalance,
                    InitialWalletBalance = initialWalletBalance,
                    LastWalletBalance = initialWalletBalance
                };

                _statisticsRepository.CreateOrUpdateSummaryAsync(summaryData).Wait();
            }
            else
            {
                var existingSummaryData = _statisticsRepository.GetSummaryAsync(_instanceId).Result;

                //Update start values for statistics summary that already exists
                existingSummaryData.AssetOneBalance = assetOneBalance;
                existingSummaryData.AssetTwoBalance = assetTwoBalance;
                existingSummaryData.InitialWalletBalance = initialWalletBalance;
                existingSummaryData.LastWalletBalance = initialWalletBalance;

                _statisticsRepository.CreateOrUpdateSummaryAsync(existingSummaryData).Wait();
            }

            var data = new Statistics
            {
                InstanceId = _instanceId,
                InstanceType = _instanceType,
                IsStarted = true
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public void OnAlgoStopped()
        {
            var data = new Statistics
            {
                InstanceId = _instanceId,
                InstanceType = _instanceType,
                IsStarted = false
            };

            _statisticsRepository.CreateAsync(data).Wait();
        }

        public StatisticsSummary GetSummary()
        {
            return _statisticsRepository.GetSummaryAsync(_instanceId).Result;
        }
    }
}
