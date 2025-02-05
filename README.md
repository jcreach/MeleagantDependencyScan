# MeleagantDependencyScan

The purpose of this library is to provide tools to facilitate dependency injection when using dotnet.

## How to use (classic case)

### First : Decorate needed classes

#### Singleton (without interfaces)

The simplest use case is the singleton without interfaces :

```CSharp
using MeleagantDependencyScan.Attributes;

[MeleagantInjection] // By default LifeTime is intialized to Singleton, so there's no need to specify it in this case
public class AuthService
{
  // Your auth logic
}
```

#### Transient or Scoped (without interfaces)

Transient use case without interfaces :

```CSharp
using MeleagantDependencyScan.Attributes;

[MeleagantInjection(LifeTime = ServiceLifetime.Transient)]
public class AuthService
{
  // Your auth logic
}
```

Scoped use case without interfaces :

```CSharp
using MeleagantDependencyScan.Attributes;

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
using MeleagantDependencyScan.Attributes;

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

#### With interfaces (Transient)

Same as `Scope` you just need to change the `LifeTime` to `Transient`

#### With interfaces (Singleton)

Same as `Scope` you just need to change the `LifeTime` to `Singleton`.
Note that **LifeTime** is set to **Singleton by default**

### Then : Scan your assemblies

To register your classes in the `ServiceCollection` you must add the `ScanAssemblies` instruction to your `Program.cs`

#### Single assembly

```CSharp
using MeleagantDependencyScan.Extensions;

// Some logic ...

// Use this instruction
builder.Services.ScanAssemblies("MySuperAssembly");

// Before this one
var app = builder.Build();
```

#### Multiple assembly

```CSharp
using MeleagantDependencyScan.Extensions;

// Some logic ...

// Use this instruction
builder.Services.ScanAssemblies("MySuperAssembly", "MyAwesomeAssembly");

// Before this one
var app = builder.Build();
```

## How to use (keyed case)

### First : Decorate needed classes

#### With interfaces (Scoped)

Let's assume that we have an `IAuthService` interface

```CSharp
public interface IAuthService
{
  string Register(RegisterDto registerDto);
}
```

And two custom auth services that implement this interface here it's `MySuperAuthService` and `MyFabulousAuthService`. And we needed it with the scoped life time.

```CSharp
using MeleagantDependencyScan.Attributes;

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IAuthService)], Key = "Super")]
public class MySuperAuthService : IAuthService
{
  // Your auth logic
  public string Register(RegisterDto registerDto)
  {
    // Register logic
  }
}

[MeleagantInjectionKeyed(LifeTime = ServiceLifetime.Scoped, VisibleFromInterface = true, VisibleAs = [typeof(IAuthService)], Key = "Fabulous")]
public class MyFabulousAuthService : IAuthService
{
  // Your auth logic
  public string Register(RegisterDto registerDto)
  {
    // Register logic
  }
}
```
#### With interfaces (Transient)

Same as `Scope` you just need to change the `LifeTime` to `Transient`

#### With interfaces (Singleton)

Same as `Scope` you just need to change the `LifeTime` to `Singleton`.
Note that **LifeTime** is set to **Singleton by default**

### Then : Scan your assemblies

To register your classes in the `ServiceCollection` you must add the `ScanAssemblies` instruction to your `Program.cs`

#### Single assembly

```CSharp
using MeleagantDependencyScan.Extensions;

// Some logic ...

// Use this instruction
builder.Services.ScanKeyedAssemblies("MySuperAssembly");

// Before this one
var app = builder.Build();
```

#### Multiple assembly

```CSharp
using MeleagantDependencyScan.Extensions;

// Some logic ...

// Use this instruction
builder.Services.ScanKeyedAssemblies("MySuperAssembly", "MyAwesomeAssembly");

// Before this one
var app = builder.Build();
```