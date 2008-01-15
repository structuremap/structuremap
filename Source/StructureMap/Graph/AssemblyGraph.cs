using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using StructureMap.Configuration.DSL;

namespace StructureMap.Graph
{
    /// <summary>
    /// Models an assembly reference in a PluginGraph
    /// </summary>
    public class AssemblyGraph : Deployable, IComparable
    {
        #region statics

        /// <summary>
        /// Finds a string array of all the assembly files in a path.  Used
        /// by the UI
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string[] GetAllAssembliesAtPath(string folderPath)
        {
            ArrayList list = new ArrayList();

            string[] files = Directory.GetFiles(folderPath, "*dll");
            foreach (string fileName in files)
            {
                try
                {
                    Assembly assembly = Assembly.LoadFrom(fileName);
                    list.Add(assembly.GetName().Name);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }

            return (string[]) list.ToArray(typeof (string));
        }

        #endregion

        private readonly Assembly _assembly;
        private readonly string _assemblyName;
        private bool _lookForPluginFamilies = true;


        /// <summary>
        /// Creates an AssemblyGraph, traps exceptions to troubleshoot configuration issues
        /// </summary>
        /// <param name="assemblyName"></param>
        public AssemblyGraph(string assemblyName)
        {
            _assemblyName = assemblyName;

            try
            {
                _assembly = AppDomain.CurrentDomain.Load(assemblyName);
            }
            catch (Exception ex)
            {
                throw new StructureMapException(101, ex, assemblyName);
            }
        }

        public AssemblyGraph(Assembly assembly)
        {
            _assemblyName = assembly.GetName().Name;
            _assembly = assembly;
        }

        /// <summary>
        /// Short name of the Assembly
        /// </summary>
        public string AssemblyName
        {
            get { return _assemblyName; }
        }


        /// <summary>
        /// Reference to the System.Reflection.Assembly object
        /// </summary>
        public Assembly InnerAssembly
        {
            get { return _assembly; }
        }

        /// <summary>
        /// Used to control whether or not the assembly should be searched for implicit attributes
        /// </summary>
        public bool LookForPluginFamilies
        {
            get { return _lookForPluginFamilies; }
            set { _lookForPluginFamilies = value; }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            AssemblyGraph peer = (AssemblyGraph) obj;
            return AssemblyName.CompareTo(peer.AssemblyName);
        }

        #endregion

        /// <summary>
        /// Returns an array of all the CLR Type's in the Assembly that are marked as
        /// [PluginFamily]
        /// </summary>
        /// <returns></returns>
        public PluginFamily[] FindPluginFamilies()
        {
            if (_assembly == null || !LookForPluginFamilies)
            {
                return new PluginFamily[0];
            }

            List<PluginFamily> list = new List<PluginFamily>();

            Type[] exportedTypes = getExportedTypes();

            foreach (Type exportedType in exportedTypes)
            {
                if (PluginFamilyAttribute.MarkedAsPluginFamily(exportedType))
                {
                    PluginFamily family = PluginFamilyAttribute.CreatePluginFamily(exportedType);
                    list.Add(family);
                }
            }

            return list.ToArray();
        }

        private Type[] getExportedTypes()
        {
            Type[] exportedTypes;
            try
            {
                exportedTypes = _assembly.GetExportedTypes();
            }
            catch (Exception ex)
            {
                throw new StructureMapException(170, ex, AssemblyName);
            }
            return exportedTypes;
        }


        public Plugin[] FindPlugins(Predicate<Type> match)
        {
            Type[] types = FindTypes(match);
            return Array.ConvertAll<Type, Plugin>(types,
                delegate(Type type) { return Plugin.CreateImplicitPlugin(type); });
        }


        public static AssemblyGraph ContainingType<T>()
        {
            return new AssemblyGraph(typeof (T).Assembly);
        }

        public Type FindTypeByFullName(string fullName)
        {
            return _assembly.GetType(fullName, false);
        }

        public List<Registry> FindRegistries()
        {
            Type[] exportedTypes = getExportedTypes();
            List<Registry> returnValue = new List<Registry>();

            foreach (Type type in exportedTypes)
            {
                if (Registry.IsPublicRegistry(type))
                {
                    Registry registry = (Registry) Activator.CreateInstance(type);
                    returnValue.Add(registry);
                }
            }

            return returnValue;
        }

        public Type[] FindTypes(Predicate<Type> match)
        {
            return Array.FindAll(getExportedTypes(), match);
        }
    }
}