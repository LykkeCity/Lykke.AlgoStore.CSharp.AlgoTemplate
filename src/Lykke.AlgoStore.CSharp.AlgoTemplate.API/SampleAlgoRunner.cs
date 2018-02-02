using Autofac;
using Common.Log;
using Lykke.AlgoStore.CSharp.Algo.Implemention;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Core.Services;
using Lykke.AlgoStore.CSharp.AlgoTemplate.Modules;
using System;
using System.Threading.Tasks;

namespace Lykke.AlgoStore.CSharp.AlgoTemplate
{
    /// <summary>
    /// A class to run the algo workflow
    /// </summary>
    public class AlgoRunner
    {
        public static async Task Main(string[] args)
        {
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
            var builder = new ContainerBuilder();
            var serviceModule = new ServiceModule(new LogToConsole());
            serviceModule.AlgoType = typeof(DummyAlgo);
            builder.RegisterModule(serviceModule);

            var ioc = builder.Build();
            return ioc;
        }
    }
}
