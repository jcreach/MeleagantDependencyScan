using MeleagantDependencyScan.Attributes;
using MeleagantDependencyScan.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeleagantDependencyScan.Extensions;

public static class MeleagantServiceCollectionExtensions
{
    public static IServiceCollection ScanAssemblies(this IServiceCollection serviceCollection, params string[] assembliesNames)
    {
        IEnumerable<Assembly> assemblies = assembliesNames.Select(Assembly.Load);

        IDictionary<Type, MeleagantInjectionAttribute?> toInject = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => Attribute.IsDefined(t, typeof(MeleagantInjectionAttribute))).ToDictionary(t => t, t => t.GetCustomAttribute<MeleagantInjectionAttribute>());


        foreach ((Type implType, MeleagantInjectionAttribute? metadatas) in toInject)
        {
            if(metadatas is null)
                continue;

            if (metadatas.VisibleFromInterface)
            {
                if (metadatas.VisibleAs is null || !metadatas.VisibleAs.Any())
                {
                    throw new MeleagantInjectionNotVisibleByItSelfException();
                }
                else
                {
                    Type firstInterfaceType = metadatas.VisibleAs.First();
                    serviceCollection.Add(new ServiceDescriptor(firstInterfaceType, implType, metadatas.LifeTime));

                    // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                    if (metadatas.VisibleAs.Length <= 1)
                        continue;

                    foreach (Type otherInterfaceType in metadatas.VisibleAs.Skip(1))
                    {
                        serviceCollection.Add(new ServiceDescriptor(otherInterfaceType, sp => sp.GetService(firstInterfaceType)!, metadatas.LifeTime));
                    }
                }
            }
            else // Visible by itself
            {
                serviceCollection.Add(new ServiceDescriptor(implType, implType, metadatas.LifeTime));

                // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                if (metadatas.VisibleAs is null) 
                    continue;

                foreach (Type interfaceType in metadatas.VisibleAs)
                {
                    serviceCollection.Add(new ServiceDescriptor(interfaceType, sp => sp.GetService(implType)!, metadatas.LifeTime));
                }
            }
        }

        return serviceCollection;
    }
}