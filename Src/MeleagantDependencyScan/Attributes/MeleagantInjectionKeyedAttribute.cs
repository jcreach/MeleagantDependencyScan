namespace MeleagantDependencyScan.Attributes
{
    public class MeleagantInjectionKeyedAttribute : MeleagantInjectionAttribute
    {
        /// <summary>
        /// Gets or sets the service Key used to register a keyed service. Empty by default
        /// </summary>
        public string Key { get; set; } = string.Empty;
    }
}