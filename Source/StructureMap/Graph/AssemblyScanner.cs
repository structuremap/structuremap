using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;

namespace StructureMap.Graph
{
    // TODO:  redo in 3.5 w/ Lambdas
    public class AssemblyScanner
    {
        private readonly List<Assembly> _assemblies = new List<Assembly>();
        private readonly GraphLog _log;
        private readonly List<ITypeScanner> _scanners = new List<ITypeScanner>();

        public AssemblyScanner(GraphLog log)
        {
            _log = log;
        }

        public int Count
        {
            get { return _assemblies.Count; }
        }

        public void ScanForAll(PluginGraph pluginGraph)
        {
            // Don't do this for SystemScan
            scanTypes(delegate(Type type)
            {
                if (!Registry.IsPublicRegistry(type)) return;

                Registry registry = (Registry) Activator.CreateInstance(type);
                registry.ConfigurePluginGraph(pluginGraph);
            });


            findFamiliesAndPlugins(pluginGraph);
            runScanners(pluginGraph);
        }

        private void runScanners(PluginGraph graph)
        {
            Registry registry = new Registry();
            scanTypes(delegate(Type type)
            {
                foreach (ITypeScanner scanner in _scanners)
                {
                    scanner.Process(type, registry);
                }
            });

            registry.ConfigurePluginGraph(graph);
        }

        private void findFamiliesAndPlugins(PluginGraph pluginGraph)
        {
            scanTypes(delegate(Type type)
            {
                if (PluginFamilyAttribute.MarkedAsPluginFamily(type))
                {
                    pluginGraph.CreateFamily(type);
                }
            });

            scanTypes(delegate(Type type)
            {
                foreach (PluginFamily family in pluginGraph.PluginFamilies)
                {
                    family.AnalyzeTypeForPlugin(type);
                }
            });
        }


        public void ScanForStructureMapObjects(PluginGraph pluginGraph)
        {
            findFamiliesAndPlugins(pluginGraph);
        }

        private void scanTypes(Action<Type> action)
        {
            scanTypes(new Action<Type>[] {action});
        }

        private void scanTypes(IEnumerable<Action<Type>> actions)
        {
            foreach (Assembly assembly in _assemblies.ToArray())
            {
                scanTypesInAssembly(assembly, actions);
            }
        }

        private void scanTypesInAssembly(Assembly assembly, IEnumerable<Action<Type>> actions)
        {
            Type[] exportedTypes;
            try
            {
                foreach (Type type in assembly.GetExportedTypes())
                {
                    foreach (Action<Type> action in actions)
                    {
                        action(type);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.RegisterError(170, ex, assembly.FullName);
            }
        }

        public void Add(Assembly assembly)
        {
            if (!_assemblies.Contains(assembly))
            {
                _assemblies.Add(assembly);
            }
        }

        public void Add(string assemblyName)
        {
            Add(AppDomain.CurrentDomain.Load(assemblyName));
        }

        public bool Contains(string assemblyName)
        {
            foreach (Assembly assembly in _assemblies)
            {
                if (assembly.GetName().Name == assemblyName)
                {
                    return true;
                }
            }

            return false;
        }

        public void AddScanner(ITypeScanner scanner)
        {
            _scanners.Add(scanner);
        }
    }
}