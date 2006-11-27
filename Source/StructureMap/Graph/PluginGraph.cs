using System;
using System.Collections;

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

		/// <summary>
		/// Default constructor
		/// </summary>
		public PluginGraph() : base()
		{
			_assemblies = new AssemblyGraphCollection(this);
			_pluginFamilies = new PluginFamilyCollection(this);
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
			if (_sealed == false)
			{
				foreach (AssemblyGraph assembly in _assemblies)
				{
					addImplicitPluginFamilies(assembly);
				}

				foreach (PluginFamily family in _pluginFamilies)
				{
					foreach (AssemblyGraph assembly in _assemblies)
					{
						family.SearchAssemblyGraph(assembly);
					}
				}

				_sealed = true;
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

	}
}