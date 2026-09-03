using MeleagantDependencyScan.Attributes;
using Microsoft.Extensions.DependencyInjection;

namespace MeleagantDependencyScan.UnitTest.TestServices;

internal interface IHelloWorldKeyedServices
{
    string SayHiKeyed(string fromWho);
}

internal interface IKeyedMockGenericInterface<TRequest, TResponse>
{
    string Execute(TRequest request);
}

[MeleagantInjectionKeyed(
    LifeTime = ServiceLifetime.Scoped,
    VisibleFromInterface = true,
    VisibleAs = [typeof(IKeyedMockGenericInterface<,>)],
    Key = "GenericKey")]
public class KeyedMockGenericClass<TRequest, TResponse> : IKeyedMockGenericInterface<TRequest, TResponse>
{
    public string Execute(TRequest request)
    {
        return $"Executed keyed {request}";
    }
}

internal interface IHelloWorldKeyedSecondaryServices
{
    string SayHiKeyedSecondary(string fromWho);
}

[MeleagantInjectionKeyed(VisibleFromInterface = true, VisibleAs = [typeof(IHelloWorldKeyedServices), typeof(IHelloWorldKeyedSecondaryServices)], Key = "MultiKey")]
public class HelloWorldKeyedMultiInterfaceServices : IHelloWorldKeyedServices, IHelloWorldKeyedSecondaryServices
{
    public string SayHiKeyed(string fromWho = "Multi")
    {
        return $"Hello Keyed World {fromWho} with interface";
    }

    public string SayHiKeyedSecondary(string fromWho = "Multi")
    {
        return $"Hello Keyed Secondary World {fromWho} with interface";
    }
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