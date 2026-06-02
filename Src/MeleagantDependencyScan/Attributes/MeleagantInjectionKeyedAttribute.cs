using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeleagantDependencyScan.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MeleagantInjectionKeyedAttribute : MeleagantInjectionAttribute
    {
        /// <summary>
        /// Gets or sets the key name of the service when the dependency is injected. Empty by default
        /// </summary>
        public string KeyName { get; set; } = string.Empty;
    }
}
