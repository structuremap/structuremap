using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;

namespace StructureMap
{
    /// <summary>
    ///     Used as the argument in the Container.Configure() method to describe
    ///     configuration directives and specify the sources of configuration for
    ///     a Container
    /// </summary>
    public class ConfigurationExpression : Registry
    {
        private readonly PluginGraphBuilder _builder = new PluginGraphBuilder();
        private readonly IList<Registry> _registries = new List<Registry>();

        internal ConfigurationExpression()
        {
            _builder.Add(this);
        }

        internal IList<Registry> Registries
        {
            get { return _registries; }
        }

        /// <summary>
        ///     Creates and adds a Registry object of type T.
        /// </summary>
        /// <typeparam name="T">The Registry Type</typeparam>
        public void AddRegistry<T>() where T : Registry, new()
        {
            AddRegistry(new T());
        }

        /// <summary>
        ///     Imports all the configuration from a Registry object
        /// </summary>
        /// <param name="registry"></param>
        public void AddRegistry(Registry registry)
        {
            _registries.Add(registry);
            _builder.Add(registry);
        }

        internal PluginGraph BuildGraph()
        {
            var pluginGraph = _builder.Build();
            return pluginGraph;
        }

        protected bool Equals(ConfigurationExpression other)
        {
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConfigurationExpression) obj);
        }

        public override int GetHashCode()
        {
            throw new System.NotImplementedException();
        }
    }
}