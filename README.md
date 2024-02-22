# MeleagantDependencyScan

The purpose of this library is to provide tools to facilitate dependency injection when using dotnet.

## How to use

### First : Decorate needed classes

#### Singleton (without interfaces)

The simplest use case is the singleton without interfaces :

```CSharp
[MeleagantInjection] // By default LifeTime is intialized to Singleton, so there's no need to specify it in this case
public class AuthService
{
  // Your auth logic
}
```

#### Transient or Scoped (without interfaces)

Transient use case without interfaces :

```CSharp
[MeleagantInjection(LifeTime = ServiceLifetime.Transient)]
public class AuthService
{
  // Your auth logic
}
```

Scoped use case without interfaces :

```CSharp
[MeleagantInjection(LifeTime = ServiceLifetime.Scoped)]
public class AuthService
{
  // Your auth logic
}
```

#### With interfaces (Scoped)

Let's assume that we have an `IAuthService` interface

```CSharp
public interface IAuthService
{
  string Register(RegisterDto registerDto);
}
```

And a custom auth service that implement this interface here it's `MySuperAuthService`. And we needed it with the scoped life time.

```CSharp
[MeleagantInjection(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IAuthService)])]
public class MySuperAuthService : IAuthService
{
  // Your auth logic
  public string Register(RegisterDto registerDto)
  {
    // Register logic
  }
}
```

### Then : Scan your assemblies

To register your classes in the `ServiceCollection` you must add the `ScanAssemblies` instruction to your `Program.cs`

#### Single assembly

```CSharp
// Use this instruction
builder.Services.ScanAssemblies("MySuperAssembly");

// Before this one
var app = builder.Build();
```

#### Multiple assembly

```CSharp
// Use this instruction
builder.Services.ScanAssemblies("MySuperAssembly", "MyAwesomeAssembly");

// Before this one
var app = builder.Build();
```
