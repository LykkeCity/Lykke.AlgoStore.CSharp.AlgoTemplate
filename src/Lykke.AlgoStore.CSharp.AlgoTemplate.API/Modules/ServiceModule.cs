using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Enumerators;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using Lykke.AlgoStore.Service.Logging.Client;
using Lykke.AlgoStore.Service.History.Client;
using Lykke.AlgoStore.Job.Stopping.Client;
using Lykke.AlgoStore.Algo;
using Lykke.AlgoStore.Service.InstanceEventHandler.Client;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Orders;
using Lykke.AlgoStore.Service.InstanceBalance.Client;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Modules
{
    /// <summary>
    /// Definition of the Autofac <see cref="Module" />
    /// </summary>
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes new instance of the <see cref="ServiceModule"/>
        /// </summary>
        /// <param name="settings">The <see cref="IReloadingManager{T}"/> implementation to be used</param>
        /// <param name="log">The <see cref="ILog"/> implementation to be used</param>
        public ServiceModule(IReloadingManager<AppSettings> settings, ILog log)
        {
            _settings = settings;
            _log = log;
            _services = new ServiceCollection();
        }

        /// <summary>
        /// The class (<see cref="Type"/>) of the algo of the <see cref="Module"/>
        /// </summary>
        public Type AlgoType { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterInstance<IStatisticsRepository>(
                    AzureRepoFactories.CreateStatisticsRepository(_settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString), _log))
                .SingleInstance();

            builder.RegisterInstance<IAlgoInstanceTradeRepository>(AzureRepoFactories.CreateAlgoTradeRepository(
                _settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString), _log)).SingleInstance();

            builder.RegisterInstance<IAlgoClientInstanceRepository>(
                    AzureRepoFactories.CreateAlgoClientInstanceRepository(
                        _settings.Nested(x => x.CSharpAlgoTemplateService.Db.TableStorageConnectionString), _log))
                .SingleInstance();

            // The algo and the algo workflow dependencies
            builder.RegisterType(AlgoType)
                .As<IAlgo>();

            builder.RegisterType<WorkflowService>()
                .As<IAlgoWorkflowService>()
                .SingleInstance();

            builder.RegisterType<CurrentDataProvider>()
                .As<ICurrentDataProvider>()
                .SingleInstance();

            builder.RegisterType<AlgoSettingsService>()
                .As<IAlgoSettingsService>()
                .SingleInstance();

            builder.RegisterType<FunctionsService>()
                .As<IFunctionsService>()
                .SingleInstance();

            builder.RegisterType<HistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<StatisticsService>()
                //.WithParameter("instanceId", _settings.CurrentValue.InstanceId)
                .As<IStatisticsService>()
                .SingleInstance();

            builder.RegisterType<ActionsService>()
                .As<IActions>();

            dynamic dynamicSettings =
                JsonConvert.DeserializeObject<ExpandoObject>(Environment.GetEnvironmentVariable("ALGO_INSTANCE_PARAMS"));

            if (dynamicSettings.InstanceType == AlgoInstanceType.Test.ToString())
            {
                builder.RegisterType<HistoricalCandleProviderService>()
                        .As<ICandleProviderService>()
                        .WithParameter(TypedParameter.From(_settings.Nested(x => x.CSharpAlgoTemplateService.CandleRabbitMqSettings)));
            }
            else
            {
                builder.RegisterType<RabbitMqCandleProviderService>()
                    .As<ICandleProviderService>()
                    .WithParameter(TypedParameter.From(_settings.Nested(x => x.CSharpAlgoTemplateService.CandleRabbitMqSettings)));
            }

            builder.RegisterType<RabbitMqQuoteProviderService>()
                .As<IQuoteProviderService>()
                .WithParameter(TypedParameter.From(_settings.Nested(x => x.CSharpAlgoTemplateService.QuoteRabbitMqSettings)));

            builder.RegisterType<TradingService>()
                .As<ITradingService>()
                .SingleInstance();

            builder.RegisterType<FakeTradingService>()
                .As<IFakeTradingService>()
                .SingleInstance();

            builder.RegisterType<CandlesService>()
                .As<ICandlesService>()
                .SingleInstance();

            builder.RegisterHistoryClient(_settings.CurrentValue.HistoryServiceClient);

            builder.RegisterType<HistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<TaskAsyncExecutor>()
                .As<IAsyncExecutor>();

            builder.RegisterType<LoggingClient>()
                .WithParameter("serviceUrl", _settings.CurrentValue.AlgoStoreLoggingServiceClient.ServiceUrl)
                .As<ILoggingClient>()
                .SingleInstance();

            builder.RegisterType<UserLogService>()
                .WithParameters(new[]
                {
                    new NamedParameter("maxBatchLifetime", TimeSpan.FromMilliseconds(_settings.CurrentValue.CSharpAlgoTemplateService.LoggingSettings.MaxBatchLifetime)),
                    new NamedParameter("batchSizeThreshold", _settings.CurrentValue.CSharpAlgoTemplateService.LoggingSettings.BatchSizeThreshold)
                })
                .As<IUserLogService>();

            builder.RegisterType<AlgoInstanceStoppingClient>()
                .As<IAlgoInstanceStoppingClient>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.AlgoStoreStoppingClient.ServiceUrl));

            builder.RegisterType<EventCollector>()
                .As<IEventCollector>()
                .WithParameter(TypedParameter.From(TimeSpan.FromMilliseconds(_settings.CurrentValue.CSharpAlgoTemplateService.EventHandlerSettings.MaxBatchLifetime)))
                .WithParameter(TypedParameter.From(_settings.CurrentValue.CSharpAlgoTemplateService.EventHandlerSettings.BatchSizeThreshold))
                .SingleInstance();

            builder.RegisterType<MonitoringService>()
                .As<IMonitoringService>()
                .WithParameter(
                TypedParameter.From(
                    TimeSpan.FromSeconds(
                        _settings.CurrentValue.CSharpAlgoTemplateService.MonitoringSettings.InstanceEventTimeoutInSec)));

            var authHandler = new AlgoAuthorizationHeaderHttpClientHandler(dynamicSettings.AuthToken);

            var instanceEventHandler = HttpClientGenerator.HttpClientGenerator
                 .BuildForUrl(_settings.CurrentValue.InstanceEventHandlerServiceClient.ServiceUrl)
                 .WithAdditionalDelegatingHandler(authHandler);

            builder.RegisterInstance(instanceEventHandler.Create().Generate<IInstanceEventHandlerClient>())
                .As<IInstanceEventHandlerClient>()
                .SingleInstance();

            builder.RegisterType<MarketOrderManager>()
                .As<IMarketOrderManager>()
                .SingleInstance();

            builder.RegisterType<LimitOrderManager>()
                .As<ILimitOrderManager>()
                .SingleInstance();

            builder.RegisterType<OrderProvider>()
                .As<IOrderProvider>()
                .SingleInstance();

            builder.RegisterType<WalletDataProvider>()
                .As<IWalletDataProvider>()
                .SingleInstance();

            builder.RegisterInstanceBalanceClient(_settings.CurrentValue.InstanceBalanceServiceClient, null);

            builder.Populate(_services);
        }
    }
}
