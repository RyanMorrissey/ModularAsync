//  Author: Ryan Morrissey
//  Date: 9/9/2018
//
//  The programs entry point

using Microsoft.Extensions.DependencyInjection;

namespace ModularAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IWorkerManager, WorkerManager>()
                .BuildServiceProvider();

            var workerManager = serviceProvider.GetService<IWorkerManager>();
            workerManager.Run();
        }
    }
}
