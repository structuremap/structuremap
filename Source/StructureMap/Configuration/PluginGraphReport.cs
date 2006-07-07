using System;
using System.Collections;
using StructureMap.Configuration.Tokens;
using StructureMap.Exceptions;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
	[Serializable]
	public class PluginGraphReport : GraphObject
	{
		private Hashtable _assemblies = new Hashtable();
		private Hashtable _families = new Hashtable();
		private InstanceDefaultManager _defaultManager;

		public PluginGraphReport()
		{

		}

		public override GraphObject[] Children
		{
			get
			{
				ArrayList list = new ArrayList();

				list.AddRange(_assemblies.Values);
				list.AddRange(_families.Values);

				list.Sort();

				return (GraphObject[]) list.ToArray(typeof (GraphObject));
			}
		}

		public InstanceDefaultManager DefaultManager
		{
			get { return _defaultManager; }
			set { _defaultManager = value; }
		}

		public void AddAssembly(AssemblyToken assemblyToken)
		{
			_assemblies.Add(assemblyToken.AssemblyName, assemblyToken);
		}

		public AssemblyToken[] Assemblies
		{
			get
			{
				AssemblyToken[] returnValue = new AssemblyToken[_assemblies.Count];
				_assemblies.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}

		public FamilyToken[] Families
		{
			get
			{
				FamilyToken[] returnValue = new FamilyToken[_families.Count];
				_families.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}

		public void AddFamily(FamilyToken family)
		{
			_families.Add(family.PluginType, family);
		}

		public FamilyToken FindFamily(string pluginTypeName)
		{
			if (!_families.ContainsKey(pluginTypeName))
			{
				throw new MissingPluginFamilyException(pluginTypeName);
			}

			return (FamilyToken) _families[pluginTypeName];
		}

		public PluginToken FindPlugin(string pluginTypeName, string concreteKey)
		{
			return FindFamily(pluginTypeName).FindPlugin(concreteKey);
		}

		public void ImportImplicitChildren(PluginGraph pluginGraph)
		{
			foreach (PluginFamily family in pluginGraph.PluginFamilies)
			{
				if (family.DefinitionSource == DefinitionSource.Implicit)
				{
					FamilyToken token = FamilyToken.CreateImplicitFamily(family);
					AddFamily(token);
				}

				addImplicitPlugins(family);
			}
		}

		private void addImplicitPlugins(PluginFamily family)
		{
			FamilyToken familyToken = FindFamily(family.PluginTypeName);

			foreach (Plugin plugin in family.Plugins)
			{
				if (plugin.DefinitionSource == DefinitionSource.Implicit)
				{
					PluginToken pluginToken = PluginToken.CreateImplicitToken(plugin);
					familyToken.AddPlugin(pluginToken);
				}
			}
		}

		public void AnalyzeInstances(PluginGraph pluginGraph)
		{
			foreach (PluginFamily family in pluginGraph.PluginFamilies)
			{
				FamilyToken token = FindFamily(family.PluginTypeName);
				token.ReadInstances(family, this);
			}
		}

		public void ValidateInstances(IInstanceValidator validator)
		{
			foreach (FamilyToken family in _families.Values)
			{
				 family.Validate(validator);
			}
		}

		public bool HasAssembly(string assemblyName)
		{
			return _assemblies.ContainsKey(assemblyName);
		}

		protected override string key
		{
			get { return string.Empty; }
		}

		public TemplateToken FindTemplate(string pluginTypeName, string templateName)
		{
			FamilyToken family = this.FindFamily(pluginTypeName);
			return family.FindTemplate(templateName);
		}
	}
}
