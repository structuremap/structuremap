using System;
using System.Collections.Generic;
using System.IO;
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

        #region IEquatable<BuildDependency> Members

        public bool Equals(BuildDependency buildDependency)
        {
            if (buildDependency == null) return false;
            return Equals(Instance, buildDependency.Instance) && Equals(PluginType, buildDependency.PluginType);
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as BuildDependency);
        }

        public override int GetHashCode()
        {
            return (Instance != null ? Instance.GetHashCode() : 0) +
                   29*(PluginType != null ? PluginType.GetHashCode() : 0);
        }
    }

    public class BuildError
    {
        private readonly List<BuildDependency> _dependencies = new List<BuildDependency>();
        private readonly Instance _instance;
        private readonly Type _pluginType;

        public BuildError(Type pluginType, Instance instance)
        {
            _instance = instance;
            _pluginType = pluginType;
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

        public StructureMapException Exception { get; set; }

        public void AddDependency(BuildDependency dependency)
        {
            if (!_dependencies.Contains(dependency))
            {
                _dependencies.Add(dependency);
            }
        }

        public void Write(StringWriter writer)
        {
            string description = ((IDiagnosticInstance) Instance).CreateToken().Description;

            writer.WriteLine();
            writer.WriteLine(
                "-----------------------------------------------------------------------------------------------------");
            writer.WriteLine("Build Error on Instance '{0}' ({1})\n    for PluginType {2}", Instance.Name, description,
                             PluginType.AssemblyQualifiedName);
            writer.WriteLine();

            if (Exception != null) writer.WriteLine(Exception.ToString());
            writer.WriteLine();
        }
    }
}