using Microsoft.Extensions.DependencyInjection;
using System;

namespace MeleagantDependencyScan.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class MeleagantInjectionAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the lifetime of the service when the dependency is injected.
    /// </summary>
    public ServiceLifetime LifeTime { get; set; }

    /// <summary>
    /// Gets or sets a boolean value indicating whether the service is visible from an interface or by the class itself. By default, the value is false (visible by the class itself).
    /// </summary>
    public bool VisibleFromInterface { get; set; } = false;

    /// <summary>
    /// Gets or sets the types of interfaces from which the service is visible. This property can be null if no types are specified.
    /// </summary>
    public Type[]? VisibleAs { get; set; }
}