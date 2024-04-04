using System;

namespace MeleagantDependencyScan.Exceptions
{
    public class MeleagantInjectionNotVisibleByItSelfException : Exception
    {
        public MeleagantInjectionNotVisibleByItSelfException(): base("This component is not visible by itself, it should be visible as at least one of the interfaces it implements")
        {
        }
    }
}