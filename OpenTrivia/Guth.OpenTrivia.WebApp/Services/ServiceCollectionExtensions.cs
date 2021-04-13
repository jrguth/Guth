using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Orleans;
using Orleans.Configuration;

namespace Guth.OpenTrivia.WebApp.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddClusterService(this IServiceCollection services, string clusterId, string serviceId)
        {
            return services
                .Configure<ClusterOptions>(opts => opts = new ClusterOptions
                {
                    ClusterId = clusterId,
                    ServiceId = serviceId
                })
                .AddSingleton<ClusterService>()
                .AddSingleton<IHostedService>(_ => _.GetService<ClusterService>())
                .AddSingleton(_ => _.GetService<ClusterService>().ClusterClient);
        }
    }
}
