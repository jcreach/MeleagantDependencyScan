using System;

namespace MeleagantDependencyScan.Exceptions
{
    public class MeleagantInjectionKeyNameNotDefinedException : Exception
    {
        public MeleagantInjectionKeyNameNotDefinedException(): base("This component is declared as keyed, key name should be defined")
        {
        }
    }
}