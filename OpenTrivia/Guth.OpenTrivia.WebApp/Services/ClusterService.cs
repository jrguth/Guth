using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Guth.OpenTrivia.GrainInterfaces;

namespace Guth.OpenTrivia.WebApp.Services
{
    public class ClusterService : IHostedService
    {
        public IClusterClient ClusterClient { get; private set; }

        public ClusterService(IOptions<ClusterOptions> options)
        {
            ClusterClient = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(opts => opts = options.Value)
                .ConfigureApplicationParts(_ => _.AddApplicationPart(typeof(IPlayerGrain).Assembly).WithReferences().WithCodeGeneration())
                .Build();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await ClusterClient.Connect(async error =>
            {
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                return true;
            });
        }

        public Task StopAsync(CancellationToken cancellationToken) => ClusterClient.Close();
    }
}
