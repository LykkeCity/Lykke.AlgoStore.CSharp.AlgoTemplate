using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain;
using static Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services.TradingService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// A service for managing the workflow of an algo execution
    /// </summary>
    public class WorkflowService : IAlgoWorkflowService
    {
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IQuoteProviderService _quoteProviderService;
        private readonly IFunctionsService _functionsService;
        private readonly IHistoryDataService _historyDataService;
        private readonly ITradingService _tradingService;
        private readonly ICandlesService _candlesService;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserLogService _logService;
        private readonly IAlgo _algo;
        private readonly ActionsService actions;
        private readonly object _sync = new object();

        public WorkflowService(
            IAlgoSettingsService algoSettingsService,
            IQuoteProviderService quoteProviderService,
            IHistoryDataService historyDataService,
            IFunctionsService functionsService,
            ITradingService tradingService,
            ICandlesService candlesService,
            IStatisticsService statisticsService,
            IUserLogService logService,
            IAlgo algo)
        {
            _algoSettingsService = algoSettingsService;
            _quoteProviderService = quoteProviderService;
            _historyDataService = historyDataService;
            _functionsService = functionsService;
            _statisticsService = statisticsService;
            _logService = logService;
            _tradingService = tradingService;
            _candlesService = candlesService;
            actions = new ActionsService(_tradingService, _statisticsService, logService, algoSettingsService, OnErrorHandler);
            _algo = algo;
        }

        public Task StartAsync()
        {
            // read settings/metadata/env. var
            _algoSettingsService.Initialize();

            var algoInstance = _algoSettingsService.GetAlgoInstance();
            var isBacktest = _algoSettingsService.GetSetting("InstanceType") == AlgoInstanceType.Test.ToString();

            //get algo parameters
            SetUpAlgoParameters(algoInstance);

            if (algoInstance != null)
            {
                algoInstance.AlgoInstanceStatus = Models.Enumerators.AlgoInstanceStatus.Started;
                algoInstance.AlgoInstanceRunDate = DateTime.UtcNow;
                _algoSettingsService.UpdateAlgoInstance(algoInstance).Wait();
            }

            // Function service initialization.
            _functionsService.Initialize();
            var candleServiceCandleRequests = _functionsService.GetCandleRequests().ToList();

            // TODO: Replace this with actual algo metadata once it's implemented
            candleServiceCandleRequests.Add(new CandleServiceRequest
            {
                AssetPair = _algo.AssetPair, //"BTCEUR",
                CandleInterval = _algo.CandleInterval,
                RequestId = _algoSettingsService.GetAlgoId(),
                StartFrom = _algo.StartFrom,
                EndOn = _algo.EndOn
            });

            _candlesService.Subscribe(candleServiceCandleRequests, OnInitialFunctionServiceData, OnFunctionServiceUpdate);

            // Gets not finished limited orders?!?
            // can we get it for algo ?!?

            _tradingService.Initialize();

            // subscribe for RabbitMQ quotes and candles
            // throws if fail
            // pass _algoSettingsService in constructor
            _candlesService.StartProducing();
            var quoteGeneration = _quoteProviderService.Initialize();

            if (!isBacktest)
            {
                _quoteProviderService.Subscribe(_algo.AssetPair, OnQuote);
                _quoteProviderService.Start();
            }

            //Update algo statistics
            _statisticsService.OnAlgoStarted();

            if (algoInstance != null)
            {
                algoInstance.AlgoInstanceStatus = AlgoInstanceStatus.Started;
                algoInstance.AlgoInstanceRunDate = DateTime.UtcNow;
                _algoSettingsService.UpdateAlgoInstance(algoInstance);
            }

            return Task.WhenAll(quoteGeneration);
        }

        public Task StopAsync()
        {
            actions.Log("Executing 'StopAsync' event started");

            _statisticsService.OnAlgoStopped();

            actions.Log("Executing 'StopAsync' event finished");

            return Task.CompletedTask;
        }

        public void OnErrorHandler(Exception e, string message)
        {
            actions.Log(e.ToString());
            actions.Log(message);
        }

        private void OnInitialFunctionServiceData(IList<MultipleCandlesResponse> warmupData)
        {
            lock (_sync)
            {
                _functionsService.WarmUp(warmupData);

                _algo.OnStartUp(_functionsService.GetFunctionResults());
            }
        }

        private void OnFunctionServiceUpdate(IList<SingleCandleResponse> candleUpdates)
        {
            var algoCandle = candleUpdates.FirstOrDefault(scr => scr.RequestId == _algoSettingsService.GetAlgoId())?.Candle;

            if (algoCandle != null && algoCandle.DateTime > _algo.EndOn)
                return;

            var ctx = CreateCandleContext(algoCandle);

            lock (_sync)
            {
                _functionsService.Recalculate(candleUpdates);

                if (algoCandle != null)
                {
                    // Allow time for all functions to recalculate before sending the event
                    Thread.Sleep(100);
                    _algo.OnCandleReceived(ctx);
                }
            }
        }

        private Task OnQuote(IAlgoQuote quote)
        {
            if (quote.DateReceived > _algo.EndOn)
                return Task.CompletedTask;

            _statisticsService.OnQuote(quote);

            var ctx = CreateQuoteContext(quote);

            try
            {
                lock (_sync)
                {
                    _algo.OnQuoteReceived(ctx);
                }
            }
            catch (TradingServiceException e)
            {
                OnErrorHandler(e);
            }

            return Task.CompletedTask;
        }

        private void OnErrorHandler(TradingServiceException e)
        {
            _logService.Write(_algoSettingsService.GetInstanceId(), e);
        }

        private IQuoteContext CreateQuoteContext(IAlgoQuote quote)
        {
            var context = new QuoteContext();

            context.Data = new AlgoQuoteData(quote);

            context.Actions = actions;

            SetContextProperties(context);

            return context;
        }

        private ICandleContext CreateCandleContext(IAlgoCandle candle)
        {
            var context = new CandleContext();

            context.Data = new AlgoCandleData(candle);

            context.Actions = actions;

            SetContextProperties(context);

            return context;
        }

        private void SetContextProperties(Context context)
        {
            context.Functions = _functionsService.GetFunctionResults();
        }

        public void SetUpAlgoParameters(AlgoClientInstanceData algoInstance)
        {
            if (algoInstance == null || algoInstance.AlgoMetaDataInformation.Parameters == null)
                return;

            Type parameterType = _algo.GetType();

            foreach (var parameter in algoInstance.AlgoMetaDataInformation.Parameters)
            {
                PropertyInfo prop = parameterType.GetProperty(parameter.Key);

                if (prop != null && prop.CanWrite)
                {
                    if (prop.PropertyType.IsEnum)
                        prop.SetValue(_algo, Enum.ToObject(prop.PropertyType, Convert.ToInt32(parameter.Value)), null);
                    else
                        prop.SetValue(_algo, Convert.ChangeType(parameter.Value, prop.PropertyType), null);
                }
            }
        }
    }
}
