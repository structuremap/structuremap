using StructureMap.Configuration.DSL;

namespace StructureMap
{
    public interface IInitializationExpression : IRegistry
    {
        /// <summary>
        /// Creates and adds a Registry object of type T.  
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        void AddRegistry<T>() where T : Registry, new();

        /// <summary>
        /// Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        void AddRegistry(Registry registry);
    }

    public class InitializationExpression : ConfigurationExpression, IInitializationExpression
    {
        internal InitializationExpression()
        {
            DefaultProfileName = string.Empty;
        }

        public string DefaultProfileName { get; set; }
    }
}