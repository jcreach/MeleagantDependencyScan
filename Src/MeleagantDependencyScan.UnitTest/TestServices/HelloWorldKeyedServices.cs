using MeleagantDependencyScan.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MeleagantDependencyScan.UnitTest.TestServices;

internal interface IHelloWorldKeyedServices
{
    string SayHiKeyed(string fromWho);
}

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Transient, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "TKeyA")]
public class HelloWorldKeyedTransientServicesWithInterfaceKeyA : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Transient KeyA")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Transient, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "TKeyB")]
public class HelloWorldKeyedTransientServicesWithInterfaceKeyB : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Transient KeyB")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "SKeyA")]
public class HelloWorldKeyedScopedServicesWithInterfaceKeyA : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Scoped KeyA")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "SKeyB")]
public class HelloWorldKeyedScopedServicesWithInterfaceKeyB : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Scoped KeyB")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}

[MeleagantInjectionKeyed(VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "SiKeyA")]
public class HelloWorldKeyedSingletonServicesWithInterfaceKeyA : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Singleton KeyA")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}

[MeleagantInjectionKeyed(VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices)], Key = "SiKeyB")]
public class HelloWorldKeyedSingletonServicesWithInterfaceKeyB : IHelloWorldKeyedServices
{
    public string SayHiKeyed(string fromWho = "Singleton KeyB")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }
}