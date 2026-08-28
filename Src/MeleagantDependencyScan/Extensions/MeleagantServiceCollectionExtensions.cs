using MeleagantDependencyScan.Attributes;
using MeleagantDependencyScan.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MeleagantDependencyScan.Extensions
{
    public static class MeleagantServiceCollectionExtensions
    {
        public static void ValidateVisibleAs(Type implType, Type[]? visibleAs)
        {
            if (visibleAs is null || !visibleAs.Any())
            {
                throw new MeleagantInjectionNotVisibleByItSelfException();
            }

            foreach (Type interfaceType in visibleAs)
            {
                if (!interfaceType.IsInterface)
                {
                    throw new InvalidOperationException($"The type '{interfaceType.Name}' in VisibleAs must be an interface. '{implType.Name}' is not valid for interface-based registration.");
                }

                bool isAssignable = interfaceType.IsAssignableFrom(implType);
                if (!isAssignable && interfaceType.IsGenericTypeDefinition && implType.IsGenericTypeDefinition)
                {
                    isAssignable = implType.GetInterfaces()
                        .Any(implementedInterface => implementedInterface.IsGenericType
                            && implementedInterface.GetGenericTypeDefinition() == interfaceType);
                }

                if (!isAssignable)
                {
                    throw new InvalidOperationException($"The type '{implType.Name}' does not implement the interface '{interfaceType.Name}' declared in VisibleAs.");
                }
            }
        }

        public static void ValidateNoDuplicateRegistration(this IServiceCollection serviceCollection, Type serviceType, Type implementationType, object? key = null)
        {
            bool isDuplicate = serviceCollection.Any(descriptor =>
                descriptor.ServiceType == serviceType &&
                ((descriptor.ImplementationType == implementationType) ||
                 (descriptor.IsKeyedService && descriptor.KeyedImplementationType == implementationType)) &&
                (key is null
                    ? !descriptor.IsKeyedService
                    : descriptor.IsKeyedService && Equals(descriptor.ServiceKey, key)));

            if (isDuplicate)
            {
                var keyDescription = key is null ? "non-keyed" : $"keyed '{key}'";
                throw new InvalidOperationException($"A duplicate registration already exists for service '{serviceType.Name}' ({keyDescription}) with implementation '{implementationType.Name}'.");
            }
        }

        private static void ValidateAssemblyNames(string[]? assembliesNames)
        {
            if (assembliesNames is null || assembliesNames.Length == 0)
            {
                throw new ArgumentException("At least one assembly name must be provided.", nameof(assembliesNames));
            }

            if (assembliesNames.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException("Assembly names cannot be null, empty, or whitespace.", nameof(assembliesNames));
            }
        }

        public static IServiceCollection ScanAssemblies(this IServiceCollection serviceCollection, params string[] assembliesNames)
        {
            ValidateAssemblyNames(assembliesNames);
            IEnumerable<Assembly> assemblies = assembliesNames.Select(Assembly.Load);

            IDictionary<Type, MeleagantInjectionAttribute?> toInject = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(MeleagantInjectionAttribute))).ToDictionary(t => t, t => t.GetCustomAttribute<MeleagantInjectionAttribute>());
            
            foreach ((Type implType, MeleagantInjectionAttribute? metadatas) in toInject)
            {
                if(metadatas is null)
                    continue;

                Type[]? visibleAs = metadatas.VisibleAs;

                if (metadatas.VisibleFromInterface)
                {
                    Type[] validatedVisibleAs = visibleAs ?? throw new MeleagantInjectionNotVisibleByItSelfException();
                    ValidateVisibleAs(implType, validatedVisibleAs);

                    Type firstInterfaceType = validatedVisibleAs[0];
                    serviceCollection.ValidateNoDuplicateRegistration(firstInterfaceType, implType);
                    serviceCollection.Add(new ServiceDescriptor(firstInterfaceType, implType, metadatas.LifeTime));

                    // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                    if (validatedVisibleAs.Length <= 1)
                        continue;

                    foreach (Type otherInterfaceType in validatedVisibleAs.Skip(1))
                    {
                        serviceCollection.ValidateNoDuplicateRegistration(otherInterfaceType, implType);
                        serviceCollection.Add(new ServiceDescriptor(otherInterfaceType, sp => sp.GetService(firstInterfaceType)!, metadatas.LifeTime));
                    }
                }
                else // Visible by itself
                {
                    serviceCollection.ValidateNoDuplicateRegistration(implType, implType);
                    serviceCollection.Add(new ServiceDescriptor(implType, implType, metadatas.LifeTime));

                    // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                    if (visibleAs is null) 
                        continue;

                    ValidateVisibleAs(implType, visibleAs);
                    foreach (Type interfaceType in visibleAs)
                    {
                        serviceCollection.ValidateNoDuplicateRegistration(interfaceType, implType);
                        serviceCollection.Add(new ServiceDescriptor(interfaceType, sp => sp.GetService(implType)!, metadatas.LifeTime));
                    }
                }
            }

            return serviceCollection;
        }

        public static IServiceCollection ScanKeyedAssemblies(this IServiceCollection serviceCollection,
            params string[] assembliesNames)
        {
            ValidateAssemblyNames(assembliesNames);
            IEnumerable<Assembly> assemblies = assembliesNames.Select(Assembly.Load);

            IDictionary<Type, MeleagantInjectionKeyedAttribute?> toInject = assemblies
                .SelectMany(a => a.GetTypes())
                .Where(t => Attribute.IsDefined(t, typeof(MeleagantInjectionKeyedAttribute))).ToDictionary(t => t, t => t.GetCustomAttribute<MeleagantInjectionKeyedAttribute>());

            foreach ((Type implType, MeleagantInjectionKeyedAttribute? metadatas) in toInject)
            {
                if(metadatas is null || string.IsNullOrWhiteSpace(metadatas.Key))
                    continue;

                Type[]? visibleAs = metadatas.VisibleAs;
                
                if (metadatas.VisibleFromInterface)
                {
                    Type[] validatedVisibleAs = visibleAs ?? throw new MeleagantInjectionNotVisibleByItSelfException();
                    ValidateVisibleAs(implType, validatedVisibleAs);

                    Type firstInterfaceType = validatedVisibleAs[0];
                    serviceCollection.ValidateNoDuplicateRegistration(firstInterfaceType, implType, metadatas.Key);
                    serviceCollection.Add(new ServiceDescriptor(firstInterfaceType, metadatas.Key, implType, metadatas.LifeTime));

                    // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                    if (validatedVisibleAs.Length <= 1)
                        continue;

                    foreach (Type otherInterfaceType in validatedVisibleAs.Skip(1))
                    {
                        serviceCollection.ValidateNoDuplicateRegistration(otherInterfaceType, implType, metadatas.Key);
                        serviceCollection.Add(new ServiceDescriptor(
                            otherInterfaceType,
                            metadatas.Key,
                            (sp, serviceKey) => sp.GetKeyedService(firstInterfaceType, serviceKey)!,
                            metadatas.LifeTime));
                    }
                }
                else // Visible by itself
                {
                    serviceCollection.ValidateNoDuplicateRegistration(implType, implType, metadatas.Key);
                    serviceCollection.Add(new ServiceDescriptor(implType, metadatas.Key, implType, metadatas.LifeTime));

                    // This ensures that if it is registered via 1 or more interfaces, the instance will always be the same.
                    if (visibleAs is null)
                        continue;

                    ValidateVisibleAs(implType, visibleAs);
                    foreach (Type interfaceType in visibleAs)
                    {
                        serviceCollection.ValidateNoDuplicateRegistration(interfaceType, implType, metadatas.Key);
                        serviceCollection.Add(new ServiceDescriptor(
                            interfaceType,
                            metadatas.Key,
                            (sp, serviceKey) => sp.GetKeyedService(implType, serviceKey)!,
                            metadatas.LifeTime));
                    }
                }
            }
            
            return serviceCollection;
        }
    }
}