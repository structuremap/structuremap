using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;

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

		private Assembly _assembly;
		private string _assemblyName;
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

		public AssemblyGraph(Assembly assembly) : base()
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
		/// Returns an array of all the CLR Type's in the Assembly that are marked as
		/// [PluginFamily]
		/// </summary>
		/// <returns></returns>
		public PluginFamily[] FindPluginFamilies()
		{
			if (_assembly == null || !this.LookForPluginFamilies)
			{
				return new PluginFamily[0];
			}

			ArrayList list = new ArrayList();

			Type[] exportedTypes = null;
			try
			{
				exportedTypes = _assembly.GetExportedTypes();
			}
			catch (Exception ex)
			{
				throw new StructureMapException(170, ex, this.AssemblyName);
			}

			foreach (Type exportedType in exportedTypes)
			{
				if (PluginFamilyAttribute.MarkedAsPluginFamily(exportedType))
				{
					PluginFamily family = PluginFamilyAttribute.CreatePluginFamily(exportedType);
					list.Add(family);
				}
			}

			return (PluginFamily[]) list.ToArray(typeof (PluginFamily));
		}

		/// <summary>
		/// Returns an array of possible Plugin's from the Assembly that match
		/// the requested type 
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public Plugin[] FindPlugins(Type type)
		{
			if (_assembly == null)
			{
				return new Plugin[0];
			}

			Plugin[] plugins = Plugin.GetPlugins(_assembly, type);
			return plugins;
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

		public int CompareTo(object obj)
		{
			AssemblyGraph peer = (AssemblyGraph) obj;
			return this.AssemblyName.CompareTo(peer.AssemblyName);
		}
	}
}