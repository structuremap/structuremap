using System;
using System.Collections;
using System.Reflection;
using StructureMap.Graph;

namespace StructureMap.Configuration.Tokens
{
	[Serializable]
	public class PluginToken : GraphObject
	{
		public static PluginToken CreateImplicitToken(Plugin plugin)
		{
			TypePath path = new TypePath(plugin.PluggedType);
			PluginToken token = new PluginToken(path, plugin.ConcreteKey, DefinitionSource.Implicit);
			token.ReadProperties(plugin);

			return token;
		}


		private string _pluggedType;
		private string _concreteKey;
		private DefinitionSource _definitionSource;
		private string _assemblyName;
		private Hashtable _properties = new Hashtable();
		private string _pluginType;

		public PluginToken() : base()
		{

		}

		public PluginToken(string pluggedType, string assemblyName, string concreteKey, DefinitionSource definitionSource)
		{
			_pluggedType = pluggedType;
			_assemblyName = assemblyName;
			_concreteKey = concreteKey;
			_definitionSource = definitionSource;
		}

		public PluginToken(TypePath pluginPath, string concreteKey, DefinitionSource definitionSource)
		{
			_pluggedType = pluginPath.AssemblyQualifiedName;
			_assemblyName = pluginPath.AssemblyName;
			_concreteKey = concreteKey;
			_definitionSource = definitionSource;
		}

		public string PluggedType
		{
			get { return _pluggedType; }
			set { _pluggedType = value; }
		}

		public string ConcreteKey
		{
			get { return _concreteKey; }
			set { _concreteKey = value; }
		}

		public DefinitionSource DefinitionSource
		{
			get { return _definitionSource; }
			set { _definitionSource = value; }
		}

		public string AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
		}

		public string PluginType
		{
			get { return _pluginType; }
			set { _pluginType = value; }
		}

		public PropertyDefinition this[string propertyName]
		{
			get
			{
				return (PropertyDefinition) _properties[propertyName];
			}
		}

		public PropertyDefinition[] Properties
		{
			get
			{
				PropertyDefinition[] returnValue = new PropertyDefinition[_properties.Count];
				_properties.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
			set
			{
				_properties = new Hashtable();
				foreach (PropertyDefinition definition in value)
				{
					_properties.Add(definition.PropertyName, definition);
				}
			}
		}

		public void AddPropertyDefinition(PropertyDefinition property)
		{
			_properties.Add(property.PropertyName, property);
		}

		public override string ToString()
		{
			return string.Format("Plugin:  {0}, ConcreteKey:  {1} ({2})", _pluggedType, _concreteKey, _definitionSource);
		}

		public override bool Equals(object obj)
		{
			PluginToken peer = obj as PluginToken;
			if (peer == null)
			{
				return false;
			}

			return this.PluggedType == peer.PluggedType && 
				this.ConcreteKey == peer.ConcreteKey && 
				this.DefinitionSource == peer.DefinitionSource && 
				this.AssemblyName == peer.AssemblyName;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void ReadProperties(Plugin plugin)
		{
			try
			{
				ConstructorInfo constructor = plugin.GetConstructor();
				PropertyDefinition[] properties = PropertyDefinitionBuilder.CreatePropertyDefinitions(constructor);
				foreach (PropertyDefinition property in properties)
				{
					this.AddPropertyDefinition(property);
				}

				foreach (SetterProperty setterProperty in plugin.Setters)
				{
					PropertyDefinition definition = PropertyDefinitionBuilder.CreatePropertyDefinition(setterProperty.Property);
					this.AddPropertyDefinition(definition);
				}
			}
			catch (Exception ex)
			{
				Problem problem = new Problem(ConfigurationConstants.PLUGIN_CANNOT_READ_CONSTRUCTOR_PROPERTIES, ex);
				LogProblem(problem);
			}
		}

		public override GraphObject[] Children
		{
			get { return Properties; }
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandlePlugin(this);
		}

		protected override string key
		{
			get { return this.ConcreteKey; }
		}

	}
}
