using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
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

            // Function service initialization.
            _functionsService.Initialize();
            var candleServiceCandleRequests = _functionsService.GetCandleRequests().ToList();
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
            _functionsService.WarmUp(warmupData);
        }

        private void OnFunctionServiceUpdate(IList<SingleCandleResponse> candleUpdates)
        {
            _functionsService.Recalculate(candleUpdates);
        }

        private Task OnQuote(IAlgoQuote quote)
        {
            // Handling of the synchronization could extract it in a separate class
            IContext ctx = CreateContext(quote);

            try
            {
                _algo.OnQuoteReceived(ctx);
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

        private IContext CreateContext(IAlgoQuote quote)
        {
            var context = new Context();

            context.Data = new ContextData(quote);

            //_functionsService.Calculate(quote);
           context.Functions = _functionsService.GetFunctionResults();

            _statisticsService.OnQuote(quote);
            context.Data = new AlgoData(quote);

            context.Actions = this.actions;

            return context;
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        public void OnErrorHandler(Exception e, string message)
        {
            throw new NotImplementedException();
        }

    }
}
