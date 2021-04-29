using System;
using System.Threading.Tasks;
using Firebase.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Guth.OpenTrivia.FirebaseDB
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTriviaDB(this IServiceCollection services, string url, string appSecret)
        {
            return services
                .AddSingleton(_ => new FirebaseClient(url, new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(appSecret)
                }))
                .AddSingleton(_ => new TriviaRealtimeDB(_.GetRequiredService<FirebaseClient>()));
        }
    }
}
