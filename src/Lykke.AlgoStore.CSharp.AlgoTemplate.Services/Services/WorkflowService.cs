using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using static Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services.TradingService;

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
        private readonly IAlgo _algo;
        private readonly ActionsService actions;
        private readonly object _sync = new object();
        private readonly string _dummyAlgoId = Guid.NewGuid().ToString();

        public WorkflowService(
            IAlgoSettingsService algoSettingsService,
            IQuoteProviderService quoteProviderService,
            IHistoryDataService historyDataService,
            IFunctionsService functionsService,
            ITradingService tradingService,
            ICandlesService candlesService,
            IStatisticsService statisticsService,
            IAlgo algo)
        {
            _algoSettingsService = algoSettingsService;
            _quoteProviderService = quoteProviderService;
            _historyDataService = historyDataService;
            _functionsService = functionsService;
            _statisticsService = statisticsService;
            _tradingService = tradingService;
            _candlesService = candlesService;
            actions = new ActionsService(_tradingService, _statisticsService, this.OnErrorHandler);
            _algo = algo;
        }



        public Task StartAsync()
        {
            // read settings/metadata/env. var
            _algoSettingsService.Initialize();

            //get algo parameters
            SetUpAlgoParameters();

            // Function service initialization.
            _functionsService.Initialize();
            var candleServiceCandleRequests = _functionsService.GetCandleRequests().ToList();

            // TODO: Replace this with actual algo metadata once it's implemented
            candleServiceCandleRequests.Add(new CandleServiceRequest
            {
                AssetPair = _algo.AssetPair, //"BTCEUR",
                CandleInterval = _algo.CandleInterval,
                RequestId = _algoSettingsService.GetAlgoId(),
                StartFrom = _algo.StartFrom
            });

            _candlesService.Subscribe(candleServiceCandleRequests, OnInitialFunctionServiceData, OnFunctionServiceUpdate);

            // Gets not finished limited orders?!?
            // can we get it for algo ?!?
            _tradingService.Initialize();

            // subscribe for RabbitMQ quotes
            // throws if fail
            // pass _algoSettingsService in constructor
            var quoteGeneration = _quoteProviderService.Initialize();
            _quoteProviderService.Subscribe(OnQuote);
            _candlesService.StartProducing();

            //Update algo statistics
            _statisticsService.OnAlgoStarted();

            return Task.WhenAll(quoteGeneration);
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
            // TODO: Replace this with actual algo metadata once it's implemented
            var algoCandle = candleUpdates.FirstOrDefault(scr => scr.RequestId == _dummyAlgoId)?.Candle;

            var ctx = CreateCandleContext(algoCandle);

            lock (_sync)
            {
                _functionsService.Recalculate(candleUpdates);

                if (algoCandle != null)
                    _algo.OnCandleReceived(ctx);
            }
        }

        private Task OnQuote(IAlgoQuote quote)
        {
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
            Console.WriteLine(e);
        }

        private IQuoteContext CreateQuoteContext(IAlgoQuote quote)
        {
            var context = new QuoteContext();

            context.Data = new AlgoQuoteData(quote);

            SetContextProperties(context);

            return context;
        }

        private ICandleContext CreateCandleContext(IAlgoCandle candle)
        {
            var context = new CandleContext();

            context.Data = new AlgoCandleData(candle);

            SetContextProperties(context);

            return context;
        }

        private void SetContextProperties(Context context)
        {
            context.Functions = _functionsService.GetFunctionResults();

            context.Actions = actions;
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public void OnErrorHandler(Exception e, string message)
        {
            throw new NotImplementedException();
        }

        private void SetUpAlgoParameters()
        {
            var algoInstance = _algoSettingsService.GetAlgoInstance();

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
