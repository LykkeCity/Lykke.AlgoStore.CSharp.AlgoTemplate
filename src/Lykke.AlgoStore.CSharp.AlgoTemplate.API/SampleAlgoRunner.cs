using Autofac;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Modules;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.Configuration;
using Lykke.AlgoStore.CSharp.Algo.Core.Domain;
using Lykke.AlgoStore.CSharp.AlgoTemplate.AzureRepositories.Mapper;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate
{
    /// <summary>
    /// A class to run the algo workflow
    /// </summary>
    public class AlgoRunner
    {
        public const string USER_DEFINED_ALGOS_NAMESPACE = "Lykke.AlgoStore.CSharp.Algo.Implemention.ExecutableClass";
        public static readonly Type DEFAULT_ALGO_CLASS_TO_RUN;

        static AlgoRunner()
        {
            // Initialize eagerly the class for the algo assembly so
            // that the runtime loads the algos assembly prior to
            // tunning the main
            DEFAULT_ALGO_CLASS_TO_RUN = typeof(DummyAlgo);
        }

        public static async Task Main(string[] args)
        {
            // Initialize AutoMapper
            Mapper.Initialize(cfg => cfg.AddProfile<AutoMapperProfile>());
            Mapper.AssertConfigurationIsValid();

            // Build the inversion of control container
            var ioc = BuildIoc();

            // Create a workflow
            var algoWorkflow = ioc.Resolve<IAlgoWorkflowService>();

            try
            {
                // Start the workflow. Await the task to block current thread on the algo execution
                await algoWorkflow.StartAsync();
            }
            catch (Exception e)
            {
                // No real handling
                Console.WriteLine($"Error '{e.Message}' was thrown while executing the algo.");
                Console.WriteLine(e);

                // Non-zero exit code
                Environment.ExitCode = 1;

                // Lets devops to see startup error in console between restarts in the Kubernetes
                var delay = TimeSpan.FromMinutes(1);

                await Task.WhenAny(
                Task.Delay(delay),
                Task.Run(() =>
                {
                    Console.ReadKey(true);
                }));
            }
        }

        /// <summary>
        /// Builds the inversion of control container <see cref="IContainer"/>
        /// </summary>
        /// <returns></returns>
        private static IContainer BuildIoc()
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .SetBasePath(Directory.GetCurrentDirectory())
                .Build();

            var appSettings = config.LoadSettings<AppSettings>();

            var builder = new ContainerBuilder();
            var serviceModule = new ServiceModule(appSettings.Nested(x => x.CSharpAlgoTemplateService), new LogToConsole());
            serviceModule.AlgoType = GetAlgoType();
            builder.RegisterModule(serviceModule);

            var ioc = builder.Build();
            return ioc;
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
