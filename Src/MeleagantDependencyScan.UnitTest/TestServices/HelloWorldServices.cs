using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeleagantDependencyScan.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MeleagantDependencyScan.UnitTest.TestServices
{
    internal interface IHelloWorldService
    {
        string SayHi(string fromWho);
    }
    public class HelloWorldServices(string fromWho)
    {
        internal string FromWho = fromWho;

        public string SayHi()
        {
            return $"Hello World {FromWho}";
        }
    }

    [MeleagantInjection(LifeTime = ServiceLifetime.Transient)]
    public class HelloWorldTransientServices() : HelloWorldServices("Transient")
    {
    }


    [MeleagantInjection(LifeTime = ServiceLifetime.Scoped)]
    public class HelloWorldScopedServices() : HelloWorldServices("Scoped")
    {
    }

    [MeleagantInjection]
    public class HelloWorldSingletonServices() : HelloWorldServices("Singleton")
    {
    }

    [MeleagantInjection(LifeTime = ServiceLifetime.Transient, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldService)])]
    public class HelloWorldTransientServicesWithInterface : IHelloWorldService
    {
        public string SayHi(string fromWho = "Transient")
        {
            return $"Hello World {fromWho} with interface";
        }
    }

    [MeleagantInjection(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldService)])]
    public class HelloWorldScopedServicesWithInterface : IHelloWorldService
    {
        public string SayHi(string fromWho = "Scoped")
        {
            return $"Hello World {fromWho} with interface";
        }
    }

    [MeleagantInjection(VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldService)])]
    public class HelloWorldSingletonServicesWithInterface : IHelloWorldService
    {
        public string SayHi(string fromWho = "Singleton")
        {
            return $"Hello World {fromWho} with interface";
        }
    }
}
