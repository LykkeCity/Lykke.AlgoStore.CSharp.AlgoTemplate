using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Candles;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Abstractions.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Strings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.MatchingEngineAdapter.Abstractions.Domain;
using Lykke.AlgoStore.MatchingEngineAdapter.Client;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Tests.Services.Services
{
    public class FakeTradingServiceTests
    {
        private readonly string _tradedAsset = "USD";
        private readonly string _oppositeAsset = "BTC";
        private readonly string _assetPair = "BTCUSD";
        private readonly double _volume = 3;
        private readonly bool _straightTrue = true;
        private readonly bool _straightFalse = true;
        private readonly string _instanceId = "test9482-2108-4b39-97ed-61ca1f4df59c";

        private readonly DateTime _dateTime = new DateTime(2018, 2, 10);

        [Test]
        public void Buy_Straight_True_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightTrue, true);
            var statisticsRepository = GetStatisticsRepositoryMock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightTrue);
            var result = service.Buy(_volume, GetIAlgoCandle());
            Assert.AreEqual(result.Result.Result, GetIAlgoCandle().Close);
        }

        [Test]
        public void Buy_Straight_False_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightFalse, true);
            var statisticsRepository = GetStatisticsRepositoryMock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightFalse);
            var result = service.Buy(_volume, GetIAlgoCandle());
            Assert.AreEqual(result.Result.Result, GetIAlgoCandle().Close);

        }

        [Test]
        public void Buy_Not_Enough_Funds_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightFalse, true);
            var statisticsRepository = GetStatisticsRepository_NotEnoughFunds_Mock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightFalse);
            var result = service.Buy(_volume, GetIAlgoCandle()).Result;

            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorMessages.NotEnoughFunds, result.Error.Message);

        }

        [Test]
        public void Sell_Not_Enough_Funds_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightFalse, true);
            var statisticsRepository = GetStatisticsRepository_NotEnoughFunds_Mock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightFalse);
            var result = service.Sell(_volume, GetIAlgoCandle()).Result;

            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorMessages.NotEnoughFunds, result.Error.Message);

        }

        [Test]
        public void Sell_Straight_True_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightTrue, false);
            var statisticsRepository = GetStatisticsRepositoryMock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightTrue);
            var result = service.Sell(_volume, GetIAlgoCandle());
            Assert.AreEqual(result.Result.Result, GetIAlgoCandle().Close);
        }

        [Test]
        public void Sell_Straight_False_Test()
        {
            var algoSettingsService = GetAlgoSettingsServiceMock();
            var algoInstanceTradeRepository = GetAlgoInstanceTradeRepositoryBuyMock(_straightTrue, false);
            var statisticsRepository = GetStatisticsRepositoryMock();

            FakeTradingService service = new FakeTradingService(algoSettingsService,
                algoInstanceTradeRepository, statisticsRepository);

            service.Initialize(_instanceId, _assetPair, _straightFalse);
            var result = service.Sell(_volume, GetIAlgoCandle());
            Assert.AreEqual(result.Result.Result, GetIAlgoCandle().Close);
        }

        [Test]
        public void Trading_Service_FakeTrading_ReturnError_Test()
        {
            IMatchingEngineAdapterClient clientMEA = new Mock<IMatchingEngineAdapterClient>().Object;
            IAlgoSettingsService settingsService = GetAlgoSettingsServiceMock();
            IFakeTradingService fakeTradingService = GetFakeTradingServiceMock();
            TradingService service = new TradingService(clientMEA, settingsService, fakeTradingService);

            var result = service.Buy(_volume).Result;

            Assert.IsNotNull(result.Error);
            Assert.AreEqual(ErrorMessages.CandleShouldNotBeNull, result.Error.Message);
        }

        #region Helper Methods

        private IAlgoSettingsService GetAlgoSettingsServiceMock()
        {
            var algoSettingsService = new Mock<IAlgoSettingsService>();

            algoSettingsService.Setup(a => a.GetTradedAsset()).Returns(_tradedAsset);
            algoSettingsService.Setup(a => a.GetAlgoInstanceOppositeAssetId()).Returns(_oppositeAsset);
            algoSettingsService.Setup(a => a.GetInstanceId()).Returns(_instanceId);
            algoSettingsService.Setup(a => a.GetAlgoInstanceAssetPairId()).Returns(_assetPair);
            algoSettingsService.Setup(a => a.GetAlgoInstanceWalletId()).Returns("testwal2-2108-4b39-97ed-61ca1f4df59c");
            algoSettingsService.Setup(a => a.IsAlgoInstanceMarketOrderStraight()).Returns(_straightTrue);
            algoSettingsService.Setup(a => a.GetAlgoInstance()).Returns(
                new AlgoClientInstanceData()
                {
                    InstanceId = _instanceId,
                    AlgoInstanceType = AlgoInstanceType.Test,
                    AssetPairId = _assetPair,
                    TradedAssetId = _tradedAsset
                });

            algoSettingsService.Setup(a => a.GetInstanceType()).Returns(AlgoInstanceType.Test);

            return algoSettingsService.Object;
        }

        private IFakeTradingService GetFakeTradingServiceMock()
        {
            var fakeTradingService = new Mock<IFakeTradingService>();

            fakeTradingService.Setup(a => a.Buy(It.IsAny<double>(), It.IsAny<IAlgoCandle>()))
                              .Returns(Task.FromResult(
                                        new ResponseModel<double>()
                                        {
                                            Result = 1000
                                        }));

            fakeTradingService.Setup(a => a.Sell(It.IsAny<double>(), It.IsAny<IAlgoCandle>()))
                              .Returns(Task.FromResult(
                                        new ResponseModel<double>()
                                        {
                                            Result = 1000
                                        }));

            return fakeTradingService.Object;
        }

        private IAlgoInstanceTradeRepository GetAlgoInstanceTradeRepositoryBuyMock(bool isStraight, bool isBuy)
        {
            var algoInstanceTradeRepository = new Mock<IAlgoInstanceTradeRepository>();

            algoInstanceTradeRepository.Setup(a => a.SaveAlgoInstanceTradeAsync(It.IsAny<AlgoInstanceTrade>()))
                .Returns((AlgoInstanceTrade trade) =>
                {
                    if (trade.AssetId == _oppositeAsset)
                        CheckAreEqualOppositeTrades(trade, isBuy, isStraight);
                    else
                    {
                        CheckAreEqualTrades(trade, isBuy);
                    }

                    return Task.CompletedTask;
                });

            return algoInstanceTradeRepository.Object;
        }

        private IStatisticsRepository GetStatisticsRepositoryMock()
        {
            var statisticsRepository = new Mock<IStatisticsRepository>();

            statisticsRepository.Setup(a => a.CreateOrUpdateSummaryAsync(It.IsAny<StatisticsSummary>()))
                .Returns(Task.CompletedTask);

            statisticsRepository.Setup(a => a.GetSummaryAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new StatisticsSummary()
                {
                    InstanceId = _instanceId,
                    LastAssetTwoBalance = 60000,
                    LastTradedAssetBalance = 50000
                }));

            return statisticsRepository.Object;
        }

        private IStatisticsRepository GetStatisticsRepository_NotEnoughFunds_Mock()
        {
            var statisticsRepository = new Mock<IStatisticsRepository>();

            statisticsRepository.Setup(a => a.CreateOrUpdateSummaryAsync(It.IsAny<StatisticsSummary>()))
                .Returns(Task.CompletedTask);

            statisticsRepository.Setup(a => a.GetSummaryAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new StatisticsSummary()
                {
                    InstanceId = _instanceId,
                    LastAssetTwoBalance = 10,
                    LastTradedAssetBalance = 1
                }));

            return statisticsRepository.Object;
        }

        private AlgoInstanceTrade GetAlgoInstanceTradeBuy()
        {
            var result = GetAlgoInstanceTradeFake(true);
            result.IsBuy = true;
            return result;
        }

        private AlgoInstanceTrade GetAlgoInstanceTradeSell()
        {
            var result = GetAlgoInstanceTradeFake(false);
            result.IsBuy = false;
            return result;
        }

        private AlgoInstanceTrade GetAlgoInstanceTradeFake(bool isBuy)
        {
            return new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                AssetPairId = _assetPair,
                AssetId = _tradedAsset,
                Amount = isBuy ? _volume : -_volume,
                Price = GetIAlgoCandle().Close,
                DateOfTrade = GetIAlgoCandle().DateTime,
            };
        }

        private AlgoInstanceTrade GetAlgoInstanceOpppositeTradeFake_Straight_True(bool isbuy)
        {
            return new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                AssetPairId = _assetPair,
                AssetId = _oppositeAsset,
                Amount = isbuy ? -_volume * GetIAlgoCandle().Close : _volume * GetIAlgoCandle().Close,
                Price = GetIAlgoCandle().Close,
                DateOfTrade = GetIAlgoCandle().DateTime,
                IsBuy = isbuy
            };
        }

        private AlgoInstanceTrade GetAlgoInstanceOpppositeTradeFake_Straight_False(bool isbuy)
        {
            return new AlgoInstanceTrade()
            {
                InstanceId = _instanceId,
                AssetPairId = _assetPair,
                AssetId = _oppositeAsset,
                Amount = isbuy ? -_volume / GetIAlgoCandle().Close : _volume / GetIAlgoCandle().Close,
                Price = GetIAlgoCandle().Close,
                DateOfTrade = GetIAlgoCandle().DateTime,
                IsBuy = isbuy
            };
        }

        private void CheckAreEqualTrades(AlgoInstanceTrade entityToCheck, bool isBuy)
        {
            AlgoInstanceTrade fakeResult = isBuy ? GetAlgoInstanceTradeBuy() : GetAlgoInstanceTradeSell();

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
            string serializedFakeResult = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedFakeResult, serializedFirst);
        }

        private void CheckAreEqualOppositeTrades(AlgoInstanceTrade entityToCheck, bool isBuy, bool isStraight)
        {
            AlgoInstanceTrade fakeResult = isStraight ? GetAlgoInstanceOpppositeTradeFake_Straight_True(isBuy) : GetAlgoInstanceOpppositeTradeFake_Straight_False(isBuy);

            string serializedFirst = JsonConvert.SerializeObject(entityToCheck);
            string serializedFakeResult = JsonConvert.SerializeObject(fakeResult);

            Assert.AreEqual(serializedFakeResult, serializedFirst);
        }

        private IAlgoCandle GetIAlgoCandle()
        {
            return new Candle()
            {
                Close = 1000,
                DateTime = _dateTime,
                Open = 5000,
                High = 6000,
            };
        }

        #endregion

    }
}
