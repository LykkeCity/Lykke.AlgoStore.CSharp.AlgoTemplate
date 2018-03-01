using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Async;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Repositories;
using Lykke.Service.Assets.Client;
using Lykke.Service.FeeCalculator.Client;
using Lykke.SettingsReader;

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
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes new instance of the <see cref="ServiceModule"/>
        /// </summary>
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

            builder.RegisterInstance<IUserLogRepository>(
                    AzureRepoFactories.CreateUserLogRepository(_settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString), _log)
                )
                .SingleInstance();

            builder.RegisterInstance<IStatisticsRepository>(
                    AzureRepoFactories.CreateStatisticsRepository(_settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString), _log))
                .SingleInstance();

            builder.RegisterInstance<IAlgoClientInstanceRepository>(
                    AzureRepoFactories.CreateAlgoClientInstanceRepository(
                        _settings.Nested(x => x.CSharpAlgoTemplateService.Db.TableStorageConnectionString), _log))
                .SingleInstance();

            // The algo and the algo workflow dependencies
            builder.RegisterType(AlgoType)
                .As<IAlgo>();

            builder.RegisterType<WorkflowService>()
                .As<IAlgoWorkflowService>();

            builder.RegisterType<AlgoSettingsService>()
                .As<IAlgoSettingsService>()
                .SingleInstance();

            builder.RegisterType<FunctionsService>()
                .As<IFunctionsService>();

            builder.RegisterType<HistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<StatisticsService>()
                //.WithParameter("instanceId", _settings.CurrentValue.InstanceId)
                .As<IStatisticsService>()
                .SingleInstance();

            builder.RegisterType<ActionsService>()
                .As<IActions>();

            builder.RegisterType<RandomDataQuoteProviderService>()
                .As<IQuoteProviderService>();

            builder.RegisterType<AssetServiceDecorator>()
                .As<IAssetServiceDecorator>()
                .SingleInstance();

            builder.RegisterFeeCalculatorClient(_settings.CurrentValue.FeeCalculatorServiceClient.ServiceUrl, _log);

            _services.RegisterAssetsClient(AssetServiceSettings.Create(
                new Uri(_settings.CurrentValue.AssetsServiceClient.ServiceUrl),
                _settings.CurrentValue.CSharpAlgoTemplateService.CacheExpirationPeriod));

            builder.BindMeClient(_settings.CurrentValue.MatchingEngineClient.IpEndpoint.GetClientIpEndPoint(), socketLog: null, ignoreErrors: true);

            builder.RegisterType<MatchingEngineAdapter>()
                .As<IMatchingEngineAdapter>()
                .WithParameter(TypedParameter.From(_settings.CurrentValue.FeeSettings))
                .SingleInstance();

            builder.RegisterType<TradingService>()
                .As<ITradingService>();

            builder.RegisterType<PredefinedDataFeedCandleService>()
                .As<ICandlesService>();

            builder.RegisterType<HardCodedMovingAverageCrossFunctionInitializationService>()
                .As<IFunctionInitializationService>();

            builder.RegisterType<PredefinedHistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<TaskAsyncExecutor>()
                .As<IAsyncExecutor>();

            builder.Populate(_services);
        }
    }
}
