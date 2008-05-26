using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Diagnostics
{
    public class BuildDependency : IEquatable<BuildDependency>
    {
        public Instance Instance;
        public Type PluginType;


        public BuildDependency(Type pluginType, Instance instance)
        {
            Instance = instance;
            PluginType = pluginType;
        }


        public bool Equals(BuildDependency buildDependency)
        {
            if (buildDependency == null) return false;
            return Equals(Instance, buildDependency.Instance) && Equals(PluginType, buildDependency.PluginType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as BuildDependency);
        }

        public override int GetHashCode()
        {
            return (Instance != null ? Instance.GetHashCode() : 0) + 29*(PluginType != null ? PluginType.GetHashCode() : 0);
        }
    }

    public class BuildError
    {
        private readonly Instance _instance;
        private readonly Type _pluginType;
        private StructureMapException _exception;
        private readonly List<BuildDependency> _dependencies = new List<BuildDependency>();

        public BuildError(Type pluginType, Instance instance)
        {
            _instance = instance;
            _pluginType = pluginType;
        }

        public void AddDependency(BuildDependency dependency)
        {
            if (!_dependencies.Contains(dependency))
            {
                _dependencies.Add(dependency);
            }
        }

        public List<BuildDependency> Dependencies
        {
            get { return _dependencies; }
        }

        public Instance Instance
        {
            get { return _instance; }
        }

        public Type PluginType
        {
            get { return _pluginType; }
        }

        public StructureMapException Exception
        {
            get { return _exception; }
            set { _exception = value; }
        }
    }
}