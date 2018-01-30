using System.Threading.Tasks;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services
{
    public class WorkflowService : IStartupManager, IShutdownManager, IActions
    {
        private readonly IAlgoSettingsService _algoSettingsService;
        private readonly IQuoteProviderService _quoteProviderService;
        private readonly IFunctionsService _functionsService;
        private readonly IHistoryDataService _historyDataService;
        private readonly ITradingService _tradingService;
        private readonly IStatisticsService _statisticsService;
        private readonly IAlgo _algo;

        public WorkflowService(IAlgoSettingsService algoSettingsService,
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
            _tradingService = tradingService;
            _statisticsService = statisticsService;
        }

        public Task StartAsync()
        {
            // read settings/metadata/env. var
            _algoSettingsService.Initialise();

            // get all function settings from _algoSettingsService
            // create functions
            _functionsService.Initialise();

            var historyRequest = _functionsService.GetRequest();
            foreach (CandlesHistoryRequest candlesHistoryRequest in historyRequest)
            {
                var candles = _historyDataService.GetHistoryCandles(candlesHistoryRequest);
                _functionsService.WarmUp(candles);
            }

            // Gets not finished limited orders?!?
            // cen we get it for algo ?!?
            _tradingService.Initialise();

            // subscribe for rabbitmq quotes
            // throws if fail
            // pass _algoSettingsService in constructor
            _quoteProviderService.Initialize();

            _quoteProviderService.Subscribe(OnQuote);

            return Task.FromResult(0);
        }

        private void OnQuote(IAlgoQuote quote)
        {
            IContext ctx = CreateContext(quote);

            _algo.OnQuoteReceived(ctx);
        }

        private IContext CreateContext(IAlgoQuote quote)
        {
            var context = new Context();

            context.Data = new ContextData(quote);

            _functionsService.Calculate(quote);
            context.Functions = _functionsService;

            _statisticsService.OnQuote(quote);
            context.Data = _statisticsService;

            context.Actions = this;

            return context;
        }

        public Task StopAsync()
        {
            throw new System.NotImplementedException();
        }

        #region IActions
        public double BuyStraight(double volume)
        {
            var price = _tradingService.BuyStraight(volume);
            // TODO what if price is not executed

            if (price > 0)
            {
                _statisticsService.OnAction(true, volume);
            }

            return price;
        }

        public double BuyReverse(double volume)
        {
            throw new System.NotImplementedException();
        }

        public double SellStraight(double volume)
        {
            throw new System.NotImplementedException();
        }

        public double SellReverse(double volume)
        {
            throw new System.NotImplementedException();
        }

        public void Log(string message)
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}
