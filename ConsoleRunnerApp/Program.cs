using ConsoleRunnerLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ConsoleRunnerApp
{
    internal class Program
    {
        //Host: Microsoft.Extensions.Hosting
        static void Main(string[] args)
        {
            //create host
            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddConsoleRunnerLib();
                }).Build();



            IServiceProvider serviceProvider = host.Services;

            serviceProvider.GetConsoleRunner();
        }


    }
}
