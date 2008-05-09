using System;
using System.Reflection;
using StructureMap.Diagnostics;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

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
        private readonly AssemblyScanner _assemblies;
        private readonly InterceptorLibrary _interceptorLibrary = new InterceptorLibrary();
        private readonly GraphLog _log = new GraphLog();
        private readonly PluginFamilyCollection _pluginFamilies;
        private readonly ProfileManager _profileManager = new ProfileManager();
        private readonly bool _useExternalRegistries = true;
        private bool _sealed = false;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PluginGraph() : base()
        {
            _assemblies = new AssemblyScanner(_log);
            _pluginFamilies = new PluginFamilyCollection(this);
        }


        public PluginGraph(bool useExternalRegistries) : this()
        {
            _useExternalRegistries = useExternalRegistries;
        }

        public AssemblyScanner Assemblies
        {
            get { return _assemblies; }
        }

        public PluginFamilyCollection PluginFamilies
        {
            get { return _pluginFamilies; }
        }

        public ProfileManager ProfileManager
        {
            get { return _profileManager; }
        }

        public GraphLog Log
        {
            get { return _log; }
        }

        #region seal

        public bool IsSealed
        {
            get { return _sealed; }
        }

        public InterceptorLibrary InterceptorLibrary
        {
            get { return _interceptorLibrary; }
        }

        public int FamilyCount
        {
            get { return _pluginFamilies.Count; }
        }

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

            if (_useExternalRegistries)
            {
                _assemblies.ScanForAll(this);
            }
            else
            {
                _assemblies.ScanForStructureMapObjects(this);
            }

            foreach (PluginFamily family in _pluginFamilies)
            {
                family.Seal();
            }

            _profileManager.Seal(this);

            _sealed = true;
        }

        #endregion

        public static PluginGraph BuildGraphFromAssembly(Assembly assembly)
        {
            PluginGraph pluginGraph = new PluginGraph();
            pluginGraph.Assemblies.Add(assembly);

            pluginGraph.Seal();

            return pluginGraph;
        }


        private void buildFamilyIfMissing(Type pluginType)
        {
            if (!_pluginFamilies.Contains(pluginType))
            {
                PluginFamily family = new PluginFamily(pluginType);
                _pluginFamilies.Add(family);
            }
        }

        public PluginFamily FindFamily(Type pluginType)
        {
            buildFamilyIfMissing(pluginType);
            return PluginFamilies[pluginType];
        }

        public bool ContainsFamily(Type pluginType)
        {
            return _pluginFamilies.Contains(pluginType);
        }

        public void CreateFamily(Type pluginType)
        {
            // Just guarantee that this PluginFamily exists
            FindFamily(pluginType);
        }
    }
}