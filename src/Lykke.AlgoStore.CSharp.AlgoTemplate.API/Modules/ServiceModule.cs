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

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Modules
{
    /// <summary>
    /// Definition of the Autofac <see cref="Module" />
    /// </summary>
    public class ServiceModule : Module
    {
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes new instance of the <see cref="ServiceModule"/>
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> implementation to be used</param>
        public ServiceModule(ILog log)
        {
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

            // The algo and the algo workflow dependencies
            builder.RegisterType(AlgoType)
                .As<IAlgo>();

            builder.RegisterType<WorkflowService>()
                .As<IAlgoWorkflowService>();

            builder.RegisterType<AlgoSettingsService>()
                .As<IAlgoSettingsService>();

            builder.RegisterType<FunctionsService>()
                .As<IFunctionsService>();

            builder.RegisterType<HistoryDataService>()
                .As<IHistoryDataService>();

            builder.RegisterType<StatisticsService>()
                .As<IStatisticsService>();

            builder.RegisterType<ActionsService>()
                .As<IActions>();

            builder.RegisterType<RandomDataQuoteProviderService>()
                .As<IQuoteProviderService>();

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
