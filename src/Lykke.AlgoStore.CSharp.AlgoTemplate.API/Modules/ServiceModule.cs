using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Services.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Repositories;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings.ServiceSettings;
using Lykke.SettingsReader;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate.Modules
{
    /// <summary>
    /// Definition of the Autofac <see cref="Module" />
    /// </summary>
    public class ServiceModule : Module
    {
        private readonly IReloadingManager<CSharpAlgoTemplateSettings> _settings;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initializes new instance of the <see cref="ServiceModule"/>
        /// </summary>
        /// <param name="log">The <see cref="ILog"/> implementation to be used</param>
        /// <param name="settings">The <see cref="IReloadingManager{T}"/> implementation to be used</param>
        public ServiceModule(IReloadingManager<CSharpAlgoTemplateSettings> settings, ILog log)
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
                    AzureRepoFactories.CreateStatisticsRepository(_settings.Nested(x => x.Db.LogsConnString), _log))
                .SingleInstance();

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
                .WithParameter("instanceId", _settings.Nested(x => x.InstanceId))
                .As<IStatisticsService>();

            builder.RegisterType<ActionsService>()
                .As<IActions>();

            builder.RegisterType<RandomDataQuoteProviderService>()
                .As<IQuoteProviderService>();

            builder.RegisterType<TradingService>()
                .As<ITradingService>();

            builder.Populate(_services);
        }
    }
}
