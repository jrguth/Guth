using System;
using System.Threading.Tasks;
using Firebase.Database;
using Microsoft.Extensions.DependencyInjection;

namespace Guth.OpenTrivia.Firebase
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFirebase(this IServiceCollection services, string url, string appSecret)
        {
            return services.AddSingleton(_ => new FirebaseClient(url, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(appSecret)
            }));
        }
    }
}
