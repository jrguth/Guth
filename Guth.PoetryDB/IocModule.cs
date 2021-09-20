using Autofac;

using RestSharp;

namespace Guth.PoetryDB
{
    class IocModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(context => new PoetryDBClient(new RestClient("https://poetrydb.org")))
                .As<IPoetryDBClient>();
        }
    }
}
