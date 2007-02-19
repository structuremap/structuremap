using System;
using System.Collections;
using System.Reflection;
using StructureMap.Configuration;

namespace StructureMap.Graph
{
    /// <summary>
    /// A PluginGraph models the entire listing of all PluginFamily’s and Plugin’s controlled by 
    /// StructureMap within the current AppDomain. The scope of the PluginGraph is controlled by 
    /// a combination of custom attributes and the StructureMap.config class
    /// </summary>
    [Serializable]
    public class PluginGraph
    {
        private AssemblyGraphCollection _assemblies;
        private bool _sealed = false;
        private PluginFamilyCollection _pluginFamilies;
        private InstanceDefaultManager _defaultManager = new InstanceDefaultManager();
        private PluginGraphReport _report = new PluginGraphReport();

        /// <summary>
        /// Default constructor
        /// </summary>
        public PluginGraph() : base()
        {
            _assemblies = new AssemblyGraphCollection(this);
            _pluginFamilies = new PluginFamilyCollection(this);

            _report.DefaultManager = _defaultManager;
        }


        public AssemblyGraphCollection Assemblies
        {
            get { return _assemblies; }
        }

        public PluginFamilyCollection PluginFamilies
        {
            get { return _pluginFamilies; }
        }

        #region seal

        /// <summary>
        /// Closes the PluginGraph for adding or removing members.  Searches all of the
        /// AssemblyGraph's for implicit Plugin and PluginFamily's
        /// </summary>
        public void Seal()
        {
            if (_sealed)
            {
                return;
            }

            foreach (AssemblyGraph assembly in _assemblies)
            {
                addImplicitPluginFamilies(assembly);
            }

            foreach (PluginFamily family in _pluginFamilies)
            {
                attachImplicitPlugins(family);
            }

            _sealed = true;
        }

        private void attachImplicitPlugins(PluginFamily family)
        {
            foreach (AssemblyGraph assembly in _assemblies)
            {
                family.SearchAssemblyGraph(assembly);
            }
        }


        private void addImplicitPluginFamilies(AssemblyGraph assemblyGraph)
        {
            PluginFamily[] families = assemblyGraph.FindPluginFamilies();

            foreach (PluginFamily family in families)
            {
                // Do not replace an explicitly defined PluginFamily with the implicit version
                if (!_pluginFamilies.Contains(family.PluginType))
                {
                    _pluginFamilies.Add(family);
                }
            }
        }


        public bool IsSealed
        {
            get { return _sealed; }
        }

        public InstanceDefaultManager DefaultManager
        {
            get { return _defaultManager; }
        }

        /// <summary>
        /// Un-seals a PluginGraph.  Makes  the PluginGraph editable
        /// </summary>
        public void UnSeal()
        {
            _sealed = false;

            ArrayList list = new ArrayList(_pluginFamilies);
            foreach (PluginFamily family in list)
            {
                if (family.DefinitionSource == DefinitionSource.Implicit)
                {
                    _pluginFamilies.Remove(family);
                }
                else
                {
                    family.RemoveImplicitChildren();
                }
            }
        }

        #endregion

        public static PluginGraph BuildGraphFromAssembly(Assembly assembly)
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(assembly);

            pluginGraph.Seal();

            return pluginGraph;
        }

        public PluginGraphReport Report
        {
            get { return _report; }
            set { _report = value; }
        }

        public TypePath LocateOrCreateFamilyForType(string fullName)
        {
            Type pluginType = findTypeByFullName(fullName);
            if (!_pluginFamilies.Contains(pluginType))
            {
                PluginFamily family = _pluginFamilies.Add(pluginType, string.Empty);
                attachImplicitPlugins(family);
            }

            return new TypePath(pluginType);
        }

        private Type findTypeByFullName(string fullName)
        {
            foreach (AssemblyGraph assembly in _assemblies)
            {
                Type type = assembly.FindTypeByFullName(fullName);
                if (type != null)
                {
                    return type;
                }
            }

            throw new StructureMapException(300, fullName);
        }

        public void ReadDefaults()
        {
            _defaultManager.ReadDefaultsFromPluginGraph(this);
        }
    }
}