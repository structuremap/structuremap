using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

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

        public override string ToString()
        {
            var format = Instance.HasExplicitName()
                ? "Instance '{0}' ({2}) for PluginType {1}"
                : "Instance '{0}' for PluginType {1}";

            return format.ToFormat(Instance.Description, PluginType, Instance.Name);
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

        public Guid RootInstance
        {
            get
            {
                return Exception.Instances.FirstOrDefault();
            }
        }

        public void AddDependency(BuildDependency dependency)
        {
            if (!_dependencies.Contains(dependency))
            {
                _dependencies.Add(dependency);
            }
        }

        public void Write(StringWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine(
                "-----------------------------------------------------------------------------------------------------");

            var format = Instance.HasExplicitName()
                ? "Build Error on Instance '{0}' ({2})\n    for PluginType {1}"
                : "Build Error on Instance '{0}' \n    for PluginType {1}";

            

            writer.WriteLine(format, Instance.Description, PluginType.GetFullName(), Instance.Name);
            _dependencies.Each(x => writer.WriteLine(" - and " + x));
            writer.WriteLine();

            if (Exception != null)
            {
                if (Exception is StructureMapBuildPlanException || Exception is StructureMapConfigurationException)
                {
                    writer.WriteLine(Exception.Title);
                    writer.WriteLine(Exception.Context);
                }
                else
                {
                    writer.WriteLine(Exception.ToString());
                }
                
            }
            writer.WriteLine();
        }
    }
}