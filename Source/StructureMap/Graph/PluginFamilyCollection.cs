using System;
using System.Collections;

namespace StructureMap.Graph
{
	/// <summary>
	/// Custom collection class for PluginFamily's
	/// </summary>
	public class PluginFamilyCollection : PluginGraphObjectCollection
	{
		private Hashtable _pluginFamilies;

		public PluginFamilyCollection(PluginGraph pluginGraph) : base(pluginGraph)
		{
			_pluginFamilies = new Hashtable();
		}

		protected override ICollection innerCollection
		{
			get { return _pluginFamilies.Values; }
		}

		public void Add(Type pluginType, string defaultInstanceKey)
		{
			PluginFamily family = new PluginFamily(pluginType, defaultInstanceKey);
			Add(family);
		}


		public void Add(Type pluginType, string defaultInstanceKey, MementoSource mementoSource)
		{
			PluginFamily family = new PluginFamily(pluginType, defaultInstanceKey, mementoSource);
			Add(family);
		}

		public void Add(PluginFamily family)
		{
			verifyNotSealed();

			string key = family.PluginTypeName;
			if (_pluginFamilies.ContainsKey(key))
			{
				_pluginFamilies[key] = family;
			}
			else
			{
				_pluginFamilies.Add(key, family);
			}
		}

		public PluginFamily this[Type pluginType]
		{
			get { return (PluginFamily) _pluginFamilies[pluginType.FullName]; }
		}

		public PluginFamily this[int index]
		{
			get
			{
				PluginFamily[] families = new PluginFamily[_pluginFamilies.Count];
				this.CopyTo(families, 0);
				return families[index];
			}
		}

		public PluginFamily this[string pluginTypeName]
		{
			get { return _pluginFamilies[pluginTypeName] as PluginFamily; }
		}

		public void Remove(PluginFamily family)
		{
			_pluginFamilies.Remove(family.PluginTypeName);
		}

		public void Remove(string pluginTypeName)
		{
			_pluginFamilies.Remove(pluginTypeName);
		}


		public void FilterByDeploymentTarget(string deploymentTarget)
		{
			foreach (PluginFamily family in this)
			{
				if (!family.IsDeployed(deploymentTarget))
				{
					this.Remove(family.PluginType.FullName);
				}
			}
		}

		public bool Contains(string pluginTypeName)
		{
			return _pluginFamilies.ContainsKey(pluginTypeName);
		}

		public void RemoveImplicitChildren()
		{
			ArrayList families = new ArrayList(_pluginFamilies.Values);
			foreach (PluginFamily family in families)
			{
				if (family.DefinitionSource == DefinitionSource.Implicit)
				{
					Remove(family);
				}
				else
				{
					family.Plugins.RemoveImplicitChildren();
				}
			}
		}
	}
}