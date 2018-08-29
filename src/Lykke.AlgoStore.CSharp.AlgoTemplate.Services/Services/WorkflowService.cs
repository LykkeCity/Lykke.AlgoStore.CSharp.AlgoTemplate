using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Domain.CandleService;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services.TradingService;
using IAlgo = Lykke.AlgoStore.Algo.IAlgo;

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
        private readonly ITradingService _tradingService;
        private readonly ICandlesService _candlesService;
        private readonly IStatisticsService _statisticsService;
        private readonly IUserLogService _logService;
        private readonly IMonitoringService _monitoringService;
        private readonly IOrderProvider _orderProvider;
        private readonly ICurrentDataProvider _currentDataProvider;
        private readonly IAlgo _algo;
        private readonly ActionsService actions;
        private readonly object _sync = new object();

        private bool _isWarmUpDone;

        public WorkflowService(
            IAlgoSettingsService algoSettingsService,
            IQuoteProviderService quoteProviderService,
            IFunctionsService functionsService,
            ITradingService tradingService,
            ICandlesService candlesService,
            IStatisticsService statisticsService,
            IUserLogService logService,
            IMonitoringService monitoringService,
            IEventCollector eventCollector,
            IOrderProvider orderProvider,
            ICurrentDataProvider currentDataProvider,
            IAlgo algo)
        {
            _algoSettingsService = algoSettingsService;
            _quoteProviderService = quoteProviderService;
            _functionsService = functionsService;
            _statisticsService = statisticsService;
            _logService = logService;
            _tradingService = tradingService;
            _candlesService = candlesService;
            _monitoringService = monitoringService;
            _orderProvider = orderProvider;
            _currentDataProvider = currentDataProvider;
            actions = new ActionsService(logService, algoSettingsService);
            _algo = algo;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var algoInstance = _algoSettingsService.GetAlgoInstance();
            var isBacktest = _algoSettingsService.GetSetting("InstanceType") == AlgoInstanceType.Test.ToString();

            //get algo parameters
            SetUpAlgoParameters(algoInstance);

            if (algoInstance != null)
            {
                algoInstance.AlgoInstanceStatus = Models.Enumerators.AlgoInstanceStatus.Started;
                algoInstance.AlgoInstanceRunDate = DateTime.UtcNow;
                await _algoSettingsService.UpdateAlgoInstance(algoInstance);
            }

            var authToken = _algoSettingsService.GetAuthToken();

            var token = _monitoringService.StartAlgoEvent(
                "The instance is being stopped because OnStartUp took too long to execute.");

            try
            {
                _algo.OnStartUp();
            }
            catch (Exception e)
            {
                throw new UserAlgoException(e);
            }
            finally
            {
                token.Cancel();
            }

            var candleServiceCandleRequests = _functionsService.GetCandleRequests(authToken).ToList();

            candleServiceCandleRequests.Add(new CandleServiceRequest
            {
                AssetPair = _algo.AssetPair,
                CandleInterval = _algo.CandleInterval,
                AuthToken = authToken,
                RequestId = authToken,
                StartFrom = _algo.StartFrom,
                IgnoreHistory = true,
                EndOn = _algo.EndOn
            });

            _candlesService.Subscribe(candleServiceCandleRequests, OnInitialFunctionServiceData, OnFunctionServiceUpdate);

            // Gets not finished limited orders?!?
            // can we get it for algo ?!?

            await _tradingService.Initialize();

            // subscribe for RabbitMQ quotes and candles
            // throws if fail
            // pass _algoSettingsService in constructor
            var candlesTask = _candlesService.StartProducing(cancellationToken);
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
                await _algoSettingsService.UpdateAlgoInstance(algoInstance);
            }

            await Task.WhenAll(quoteGeneration, candlesTask);
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

                _isWarmUpDone = true;
            }
        }

        private void OnFunctionServiceUpdate(IList<SingleCandleResponse> candleUpdates)
        {
            var algoCandle = candleUpdates.FirstOrDefault(scr => scr.RequestId == _algoSettingsService.GetAuthToken())?.Candle;

            if (algoCandle != null && algoCandle.DateTime > _algo.EndOn)
                return;

            var ctx = CreateCandleContext(algoCandle);

            lock (_sync)
            {
                _functionsService.Recalculate(candleUpdates);

                if (_isWarmUpDone && algoCandle != null)
                {
                    // Allow time for all functions to recalculate before sending the event
                    Thread.Sleep(100);

                    _currentDataProvider.CurrentCandle = algoCandle;
                    _currentDataProvider.CurrentQuote = null;

                    _currentDataProvider.CurrentTimestamp = algoCandle.DateTime;
                    _currentDataProvider.CurrentPrice = algoCandle.Close;

                    var token = _monitoringService.StartAlgoEvent(
                        "The instance is being stopped because OnCandleReceived took too long to execute.");

                    try
                    {
                        _algo.OnCandleReceived(ctx);
                    }
                    catch (Exception e)
                    {
                        throw new UserAlgoException(e);
                    }
                    finally
                    {
                        token.Cancel();
                    }
                }
            }
        }

        private Task OnQuote(IAlgoQuote quote)
        {
            if (quote.DateReceived > _algo.EndOn)
                return Task.CompletedTask;

            if (!_isWarmUpDone)
                return Task.CompletedTask;

            _statisticsService.OnQuote(quote);

            var ctx = CreateQuoteContext(quote);

            lock (_sync)
            {
                _currentDataProvider.CurrentCandle = null;
                _currentDataProvider.CurrentQuote = quote;

                _currentDataProvider.CurrentTimestamp = quote.Timestamp;
                _currentDataProvider.CurrentPrice = quote.Price;

                var token = _monitoringService.StartAlgoEvent(
                    "The instance is being stopped because OnQuoteReceived took too long to execute.");

                try
                {
                    _algo.OnQuoteReceived(ctx);
                }
                catch (Exception e)
                {
                    throw new UserAlgoException(e);
                }
                finally
                {
                    token.Cancel();
                }
            }

            return Task.CompletedTask;
        }

        private void OnErrorHandler(TradingServiceException e)
        {
            _logService.Enqueue(_algoSettingsService.GetInstanceId(), e.ToString());
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
            context.Orders = _orderProvider;
        }

        public void SetUpAlgoParameters(AlgoClientInstanceData algoInstance)
        {
            if (algoInstance == null || algoInstance.AlgoMetaDataInformation.Parameters == null)
                return;

            Type parameterType = _algo.GetType();

            var baseAlgo = parameterType.BaseType;
            baseAlgo.GetField("_paramProvider", BindingFlags.Instance | BindingFlags.NonPublic)
                    .SetValue(_algo, _functionsService);

            foreach (var parameter in algoInstance.AlgoMetaDataInformation.Parameters)
            {
                var currentType = parameterType;
                FieldInfo field = null;

                while (field == null && currentType != null)
                {
                    field = currentType
                        .GetField($"<{parameter.Key}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                    currentType = currentType.BaseType;
                }

                if (field != null)
                {
                    if (field.FieldType.IsEnum)
                        field.SetValue(_algo, Enum.ToObject(field.FieldType, Convert.ToInt32(parameter.Value)));
                    else
                        field.SetValue(_algo, Convert.ChangeType(parameter.Value, field.FieldType));
                }
            }
        }
    }
}
