using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.Hosting;
using Orleans.Configuration;
using Guth.OpenTrivia.Grains;
using Guth.OpenTrivia.Abstractions;
using Guth.OpenTrivia.Client;

namespace Guth.OpenTrivia.Silo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new HostBuilder()
                .UseOrleans(ConfigureSilo)
                .ConfigureLogging(logging => logging
                    .AddConsole()
                    .AddFilter("Orleans.Runtime.Management.ManagementGrain", LogLevel.Warning)
                    .AddFilter("Orleans.Runtime.SiloControl", LogLevel.Warning))
                .ConfigureServices(services =>
                {
                    services.AddTransient<IOpenTriviaClient, OpenTriviaClient>();
                })
                .Build()
                .RunAsync();
        }

        private static void ConfigureSilo(ISiloBuilder builder)
        {
            builder
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(opts =>
                {
                    opts.ClusterId = "guth-open-trivia-cluster";
                    opts.ServiceId = "Guth.OpenTrivia.Silo";
                })
                .Configure<EndpointOptions>(opts =>
                {
                    opts.AdvertisedIPAddress = IPAddress.Loopback;
                })
                .ConfigureApplicationParts(_ => _.AddApplicationPart(typeof(GameGrain).Assembly).WithReferences().WithCodeGeneration())
                .AddMemoryGrainStorageAsDefault()
                .UseDashboard();
        }
    }
}
