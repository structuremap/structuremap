using System.Collections;
using System.Reflection;

namespace StructureMap.Graph
{
    /// <summary>
    /// Custom collection for AssemblyGraph's
    /// </summary>
    public class AssemblyGraphCollection : PluginGraphObjectCollection
    {
        private Hashtable _assemblies;

        public AssemblyGraphCollection(PluginGraph pluginGraph) : base(pluginGraph)
        {
            _assemblies = new Hashtable();
        }

        protected override ICollection innerCollection
        {
            get { return _assemblies.Values; }
        }

        public AssemblyGraph Add(string assemblyName)
        {
            AssemblyGraph assemblyGraph = new AssemblyGraph(assemblyName);
            return Add(assemblyGraph);
        }

        public AssemblyGraph Add(Assembly assembly)
        {
            return Add(new AssemblyGraph(assembly));
        }

        public AssemblyGraph Add(AssemblyGraph assemblyGraph)
        {
            verifyNotSealed();

            _assemblies.Add(assemblyGraph.AssemblyName, assemblyGraph);
            return assemblyGraph;
        }

        public AssemblyGraph this[string assemblyName]
        {
            get { return _assemblies[assemblyName] as AssemblyGraph; }
        }

        public AssemblyGraph this[int index]
        {
            get
            {
                AssemblyGraph[] array = new AssemblyGraph[_assemblies.Count];
                _assemblies.Values.CopyTo(array, 0);
                return array[index];
            }
        }

        public void Remove(string assemblyName)
        {
            verifyNotSealed();
            _assemblies.Remove(assemblyName);
        }

        public void Remove(AssemblyGraph assemblyGraph)
        {
            Remove(assemblyGraph.AssemblyName);
        }

        public bool Contains(string assemblyName)
        {
            return _assemblies.ContainsKey(assemblyName);
        }
    }
}