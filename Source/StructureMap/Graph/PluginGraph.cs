using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Configuration.DSL;
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
        private readonly InterceptorLibrary _interceptorLibrary = new InterceptorLibrary();
        private readonly List<Type> _pluggedTypes = new List<Type>();
        private readonly PluginFamilyCollection _pluginFamilies;
        private readonly ProfileManager _profileManager = new ProfileManager();
        private readonly List<Registry> _registries = new List<Registry>();
        private readonly List<AssemblyScanner> _scanners = new List<AssemblyScanner>();
        private GraphLog _log = new GraphLog();
        private bool _sealed;


        /// <summary>
        /// Default constructor
        /// </summary>
        public PluginGraph()
        {
            _pluginFamilies = new PluginFamilyCollection(this);
        }

        public PluginGraph(AssemblyScanner assemblies)
        {
            _pluginFamilies = new PluginFamilyCollection(this);
            _scanners.Add(assemblies);
        }

        public void Scan(Action<AssemblyScanner> action)
        {
            var scanner = new AssemblyScanner();
            action(scanner);

            AddScanner(scanner);
        }

        public void ScanThisAssembly()
        {
            Scan(x => x.TheCallingAssembly());
        }

        public List<Registry> Registries
        {
            get { return _registries; }
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
            set { _log = value; }
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

            _scanners.ForEach(scanner => scanner.ScanForAll(this));

            _pluginFamilies.Each(family => family.AddTypes(_pluggedTypes));
            _pluginFamilies.Each(family => family.Seal());

            _profileManager.Seal(this);

            _sealed = true;
        }

        #endregion

        public void AddScanner(AssemblyScanner scanner)
        {
            _scanners.Add(scanner);
        }

        public static PluginGraph BuildGraphFromAssembly(Assembly assembly)
        {
            var graph = new PluginGraph();
            graph.Scan(x => x.Assembly(assembly));

            graph.Seal();

            return graph;
        }

        public PluginFamily FindFamily(Type pluginType)
        {
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

        public void SetDefault(string profileName, Type pluginType, Instance instance)
        {
            FindFamily(pluginType).AddInstance(instance);
            _profileManager.SetDefault(profileName, pluginType, instance);
        }

        public void AddType(Type pluginType, Type concreteType)
        {
            FindFamily(pluginType).AddType(concreteType);
        }

        public void AddType(Type pluggedType)
        {
            _pluggedTypes.Add(pluggedType);
        }
    }
}