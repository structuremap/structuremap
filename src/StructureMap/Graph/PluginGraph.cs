using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Graph
{
    public interface IPluginGraph
    {
        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        void AddType(Type pluginType, Type concreteType);

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        void AddType(Type pluginType, Type concreteType, string name);

        /// <summary>
        /// Add the PluggedType as an instance to any configured pluginType where PluggedType
        /// could be assigned to the pluginType
        /// </summary>
        /// <param name="pluggedType"></param>
        void AddType(Type pluggedType);
    }


    public class WeakReference<T>
    {
        private readonly Func<T> _builder;
        private readonly WeakReference _reference;

        public WeakReference(Func<T> builder)
        {
            _builder = builder;
            _reference = new WeakReference(_builder());
        }

        public T Value
        {
            get
            {
                if (!_reference.IsAlive)
                {
                    _reference.Target = _builder();
                }

                return (T) _reference.Target;
            }
        }
    }


    /// <summary>
    /// Models the runtime configuration of a StructureMap Container
    /// </summary>
    [Serializable]
    public class PluginGraph : IPluginGraph
    {
        private readonly InterceptorLibrary _interceptorLibrary = new InterceptorLibrary();
        private readonly List<Type> _pluggedTypes = new List<Type>();
        private readonly PluginFamilyCollection _pluginFamilies;
        private readonly ProfileManager _profileManager = new ProfileManager();
        private readonly List<Registry> _registries = new List<Registry>();
        private readonly List<AssemblyScanner> _scanners = new List<AssemblyScanner>();
        private readonly WeakReference<TypePool> _types;
        private GraphLog _log = new GraphLog();
        private bool _sealed;


        public PluginGraph()
        {
            _pluginFamilies = new PluginFamilyCollection(this);
            _types = new WeakReference<TypePool>(() => new TypePool(this));
        }

        public TypePool Types { get { return _types.Value; } }

        public List<Registry> Registries { get { return _registries; } }

        public PluginFamilyCollection PluginFamilies { get { return _pluginFamilies; } }

        public ProfileManager ProfileManager { get { return _profileManager; } }

        public GraphLog Log { get { return _log; } set { _log = value; } }

        #region seal

        /// <summary>
        /// Designates whether a PluginGraph has been "Sealed."
        /// </summary>
        public bool IsSealed { get { return _sealed; } }

        public InterceptorLibrary InterceptorLibrary { get { return _interceptorLibrary; } }

        public int FamilyCount { get { return _pluginFamilies.Count; } }

        /// <summary>
        /// Closes the PluginGraph for adding or removing members.  Runs all the <see cref="AssemblyScanner"> AssemblyScanner's</see>
        /// and attempts to attach concrete types to the proper plugin types.  Calculates the Profile defaults. 
        /// </summary>
        public void Seal()
        {
            if (_sealed)
            {
                return;
            }

            // This was changed to support .Net 4.5 which is stricture on collection modification	
            int index = 0;
            while (index < _scanners.Count())
            {
                _scanners[index++].As<IPluginGraphConfiguration>().Configure(this);
            }

            _pluginFamilies.Each(family => family.AddTypes(_pluggedTypes));
            _pluginFamilies.Each(family => family.Seal());

            _profileManager.Seal(this);

            _sealed = true;
        }

        #endregion

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        public virtual void AddType(Type pluginType, Type concreteType)
        {
            FindFamily(pluginType).AddType(concreteType);
        }

        /// <summary>
        /// Adds the concreteType as an Instance of the pluginType with a name
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="concreteType"></param>
        /// <param name="name"></param>
        public virtual void AddType(Type pluginType, Type concreteType, string name)
        {
            FindFamily(pluginType).AddType(concreteType, name);
        }

        /// <summary>
        /// Add the PluggedType as an instance to any configured pluginType where PluggedType
        /// could be assigned to the pluginType
        /// </summary>
        /// <param name="pluggedType"></param>
        public virtual void AddType(Type pluggedType)
        {
            // TODO -- let's get this moved out of PluginGraph and into
            // a separate IPluginGraphConfiguration object
            _pluggedTypes.Add(pluggedType);
        }

        /// <summary>
        /// Adds an AssemblyScanner to the PluginGraph.  Used for Testing.
        /// </summary>
        /// <param name="action"></param>
        public void Scan(Action<AssemblyScanner> action)
        {
            var scanner = new AssemblyScanner();
            action(scanner);

            AddScanner(scanner);
        }

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

        /// <summary>
        /// Add configuration to a PluginGraph with the Registry DSL
        /// </summary>
        /// <param name="action"></param>
        public void Configure(Action<Registry> action)
        {
            var registry = new Registry();
            action(registry);

            registry.As<IPluginGraphConfiguration>().Configure(this);
        }

        public void ImportRegistry(Type type)
        {
            if (Registries.Any(x => x.GetType() == type)) return;

            var registry = (Registry) Activator.CreateInstance(type);
            registry.As<IPluginGraphConfiguration>().Configure(this);
        }
    }
}