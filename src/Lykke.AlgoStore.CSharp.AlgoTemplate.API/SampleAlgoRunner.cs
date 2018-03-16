using Autofac;
using AutoMapper;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Models.Mapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Modules;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using AzureStorage.Tables;
using Lykke.Logs;
using Lykke.SlackNotification.AzureQueue;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate
{
    /// <summary>
    /// A class to run the algo workflow
    /// </summary>
    public class AlgoRunner
    {
        public const string USER_DEFINED_ALGOS_NAMESPACE = "Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass";
        public static readonly Type DEFAULT_ALGO_CLASS_TO_RUN;
        private static ILog _log;
        private static IAlgoWorkflowService _algoWorkflow;

        static AlgoRunner()
        {
            // Initialize eagerly the class for the algo assembly so
            // that the runtime loads the algos assembly prior to
            // tunning the main
            DEFAULT_ALGO_CLASS_TO_RUN = typeof(DummyAlgo);
        }

        public static async Task Main(string[] args)
        {
            AssemblyLoadContext.Default.Unloading += Default_Unloading;

            try
            {
                // Initialize AutoMapper
               Mapper.Initialize(cfg =>
                {
                cfg.AddProfiles(typeof(AutoMapperProfile));
                cfg.AddProfiles(typeof(AutoMapperModelProfile));
                 });
            Mapper.AssertConfigurationIsValid();
              
                var services = new ServiceCollection();

                // Build the inversion of control container
                var ioc = BuildIoc(services);

                // Create a workflow
                _algoWorkflow = ioc.Resolve<IAlgoWorkflowService>();

                // Start the workflow. Await the task to block current thread on the algo execution
                await _algoWorkflow.StartAsync();
            }
            catch (Exception e)
            {
                // No real handling
                Console.WriteLine($@"Error '{e.Message}' was thrown while executing the algo.");
                Console.WriteLine(e);

                _log?.WriteFatalErrorAsync(nameof(AlgoRunner),nameof(Main), "", e);

                // Non-zero exit code
                Environment.ExitCode = 1;

                // Lets devops to see startup error in console between restarts in the Kubernetes
                var delay = TimeSpan.FromMinutes(1);

                await Task.WhenAny(
                    Task.Delay(delay),
                    Task.Run(() => { Console.ReadKey(true); })
                );
            }
        }

        private static void Default_Unloading(AssemblyLoadContext obj)
        {
            _algoWorkflow.StopAsync();
        }

        /// <summary>
        /// Builds the inversion of control container <see cref="IContainer"/>
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private static IContainer BuildIoc(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            var builder = new ContainerBuilder();
            var appSettings = config.LoadSettings<AppSettings>();

            _log = CreateLogWithSlack(services, appSettings);

            var serviceModule = new ServiceModule(appSettings, _log);

            serviceModule.AlgoType = GetAlgoType();

            builder.RegisterModule(serviceModule);
            builder.Populate(services);

            var ioc = builder.Build();
            return ioc;
        }

        private static ILog CreateLogWithSlack(IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var consoleLogger = new LogToConsole();
            var aggregateLogger = new AggregateLogger();

            aggregateLogger.AddLog(consoleLogger);

            var dbLogConnectionStringManager = settings.Nested(x => x.CSharpAlgoTemplateService.Db.LogsConnString);
            var dbLogConnectionString = dbLogConnectionStringManager.CurrentValue;

            if (string.IsNullOrEmpty(dbLogConnectionString))
            {
                consoleLogger.WriteWarningAsync(nameof(Startup), nameof(CreateLogWithSlack), "Table logger is not initiated").Wait();
                return aggregateLogger;
            }

            if (dbLogConnectionString.StartsWith("${") && dbLogConnectionString.EndsWith("}"))
                throw new InvalidOperationException($"LogsConnString {dbLogConnectionString} is not filled in settings");

            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager(
                AzureTableStorage<LogEntity>.Create(dbLogConnectionStringManager, "CSharpAlgoTemplateLog", consoleLogger),
                consoleLogger);

            // Creating slack notification service, which logs own azure queue processing messages to aggregate log
            var slackService = services.UseSlackNotificationsSenderViaAzureQueue(new AzureQueueIntegration.AzureQueueSettings
            {
                ConnectionString = settings.CurrentValue.SlackNotifications.AzureQueue.ConnectionString,
                QueueName = settings.CurrentValue.SlackNotifications.AzureQueue.QueueName
            }, aggregateLogger);

            var slackNotificationsManager = new LykkeLogToAzureSlackNotificationsManager(slackService, consoleLogger);

            // Creating azure storage logger, which logs own messages to console log
            var azureStorageLogger = new LykkeLogToAzureStorage(
                persistenceManager,
                slackNotificationsManager,
                consoleLogger);

            azureStorageLogger.Start();

            aggregateLogger.AddLog(azureStorageLogger);

            return aggregateLogger;
        }

        private static Type GetAlgoType()
        {
            var userDefinedAlgos = AppDomain.CurrentDomain.GetAssemblies()
                   .SelectMany(t => t.GetTypes())
                   .Where(t => t.IsClass
                   && t.Namespace == USER_DEFINED_ALGOS_NAMESPACE
                   && typeof(IAlgo).IsAssignableFrom(t)).ToList();

            // Single user defined algo found
            if (userDefinedAlgos.Count == 1)
            {
                return userDefinedAlgos[0];
            }

            // More than one user defined algos found. Can not run all of them - single algo is expected
            if (userDefinedAlgos.Count > 1)
            {
                throw new Exception($"Ambiguous request for running algos. " +
                    $"Found more than one #{typeof(IAlgo).Name} implementation " +
                    $"in namespace ${USER_DEFINED_ALGOS_NAMESPACE}");
            }

            // If no user defined algos found run the default one
            return DEFAULT_ALGO_CLASS_TO_RUN;
        }
    }
}
