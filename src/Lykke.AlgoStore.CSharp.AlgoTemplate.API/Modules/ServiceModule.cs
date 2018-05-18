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
using Lykke.Service.CandlesHistory.Client;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Dynamic;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Functions.Core.Domain;

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

            builder.RegisterInstance<IUserLogRepository>(
                    AzureRepoFactories.CreateUserLogRepository(_settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString), _log)
                )
                .SingleInstance();

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
                .As<ICandleActions, IQuoteActions>();

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
                .As<ITradingService>();

            builder.RegisterType<FakeTradingService>()
                .As<IFakeTradingService>();

            builder.RegisterType<CandlesService>()
                .As<ICandlesService>();

            builder.RegisterType<FunctionInitializationService>()
                .As<IFunctionInitializationService>();

            builder.RegisterType<Candleshistoryservice>()
                .As<ICandleshistoryservice>()
                .WithParameter(TypedParameter.From(new Uri(_settings.CurrentValue.CandlesHistoryServiceClient.ServiceUrl)));

            builder.RegisterType<HistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<TaskAsyncExecutor>()
                .As<IAsyncExecutor>();

            builder.RegisterType<UserLogService>()
                .As<IUserLogService>();

            builder.Populate(_services);
        }
    }
}
