using System;
using System.Linq;

namespace Autofac
{
    public static class AutofacExtensions
    {
        public static void RegisterGuthAssemblies(this ContainerBuilder builder)
        {
            var assemblies = AppDomain.CurrentDomain
                .GetAssemblies()
                .ToList()
                .Where(a => a.FullName.StartsWith("Guth."))
                .ToArray();
            builder.RegisterAssemblyModules(assemblies.ToArray());
        }
    }
}
