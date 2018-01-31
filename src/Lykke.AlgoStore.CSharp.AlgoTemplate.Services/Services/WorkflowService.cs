using System;
using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using static Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services.TraddingService;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    /// <summary>
    /// A service for managing the workflow of an algo execution
    /// </summary>
    public class WorkflowService : IStartupManager, IShutdownManager
    {
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IQuoteProviderService _quoteProviderService;
        private readonly IFunctionsService _functionsService;
        private readonly IHistoryDataService _historyDataService;
        private readonly ITradingService _tradingService;
        private readonly IStatisticsService _statisticsService;
        private readonly IAlgo _algo;
        private readonly ActionsService actions;

        public WorkflowService(
            IAlgoSettingsService algoSettingsService,
            IQuoteProviderService quoteProviderService,
            IHistoryDataService historyDataService,
            IFunctionsService functionsService,
            ITradingService tradingService,
            IStatisticsService statisticsService,
            IAlgo algo)
        {
            _algoSettingsService = algoSettingsService;
            _quoteProviderService = quoteProviderService;
            _historyDataService = historyDataService;
            _functionsService = functionsService;
            _statisticsService = statisticsService;
            _tradingService = tradingService;
            actions = new ActionsService(_tradingService, _statisticsService, this.OnErrorHandler);
        }

        public Task StartAsync()
        {
            // read settings/metadata/env. var
            _algoSettingsService.Initialise();

            // get all function settings from _algoSettingsService
            // create functions
            _functionsService.Initialise();

            // This should be putted in a separate class.
            var historyRequest = _functionsService.GetRequest();
            foreach (CandlesHistoryRequest candlesHistoryRequest in historyRequest)
            {
                var candles = _historyDataService.GetHistoryCandles(candlesHistoryRequest);
                _functionsService.WarmUp(candles);
            }

            // Gets not finished limited orders?!?
            // can we get it for algo ?!?
            _tradingService.Initialise();

            // subscribe for RabbitMQ quotes
            // throws if fail
            // pass _algoSettingsService in constructor
            _quoteProviderService.Initialize();

            _quoteProviderService.Subscribe(OnQuote);

            return Task.FromResult(0);
        }

        private void OnQuote(IAlgoQuote quote)
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
        }

        private void OnErrorHandler(TradingServiceException e)
        {
            throw new NotImplementedException();
        }

        private IContext CreateContext(IAlgoQuote quote)
        {
            var context = new Context();

            context.Data = new ContextData(quote);

            _functionsService.Calculate(quote);
            context.Functions = _functionsService;

            _statisticsService.OnQuote(quote);
            context.Data = _statisticsService;

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
