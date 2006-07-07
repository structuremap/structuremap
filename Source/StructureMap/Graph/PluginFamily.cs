using System;
using System.IO;
using StructureMap.Interceptors;
using StructureMap.Source;

namespace StructureMap.Graph
{
	/// <summary>
	/// Conceptually speaking, a PluginFamily object represents a point of abstraction or variability in 
	/// the system.  A PluginFamily defines a CLR Type that StructureMap can build, and all of the possible
	/// Plugin’s implementing the CLR Type.
	/// </summary>
	public class PluginFamily : Deployable
	{
		#region statics

		public static PluginFamily CreateAutoFilledPluginFamily(Type pluginType)
		{
			Plugin plugin = Plugin.CreateExplicitPlugin(pluginType, CONCRETE_KEY, string.Empty);
			if (!plugin.CanBeAutoFilled)
			{
				throw new StructureMapException(231);
			}

			PluginFamily family = new PluginFamily(pluginType);
			family.DefinitionSource = DefinitionSource.Implicit;

			family.Plugins.Add(plugin);

			family.DefaultInstanceKey = CONCRETE_KEY;

			return family;
		}

		#endregion

		private string _defaultKey = string.Empty;
		private Type _pluginType;
		private MementoSource _source = new MemoryMementoSource();
		private DefinitionSource _definitionSource = DefinitionSource.Implicit;
		private string _pluginTypeName;
		private InterceptionChain _interceptionChain;
		private PluginCollection _plugins;

		public const string CONCRETE_KEY = "CONCRETE";

		#region constructors

		public PluginFamily(Type pluginType, string defaultInstanceKey) : base()
		{
			_pluginType = pluginType;
			_pluginTypeName = _pluginType.FullName;
			_defaultKey = defaultInstanceKey;
			_plugins = new PluginCollection(this);

			_interceptionChain = new InterceptionChain();
		}

		public PluginFamily(Type pluginType, string defaultInstanceKey, MementoSource source)
			: this(pluginType, defaultInstanceKey)
		{
			this.Source = source;
			_definitionSource = DefinitionSource.Explicit;
			_pluginTypeName = _pluginType.FullName;	
		}

		/// <summary>
		/// Testing constructor
		/// </summary>
		/// <param name="pluginType"></param>
		public PluginFamily(Type pluginType) :
			this(pluginType, PluginFamilyAttribute.GetDefaultKey(pluginType))
		{
			if (PluginFamilyAttribute.IsMarkedAsSingleton(pluginType))
			{
				this.InterceptionChain.AddInterceptor(new SingletonInterceptor());
			}
		}


		/// <summary>
		/// Troubleshooting constructor to find potential problems with a PluginFamily's
		/// configuration
		/// </summary>
		/// <param name="path"></param>
		/// <param name="defaultKey"></param>
		public PluginFamily(TypePath path, string defaultKey) : base()
		{
			_plugins = new PluginCollection(this);
			_pluginTypeName = path.ClassName;
			_interceptionChain = new InterceptionChain();
			initializeExplicit(path, defaultKey);
		}


		private void initializeExplicit(TypePath path, string defaultKey)
		{
			try
			{
				_pluginType = path.FindType();
				_pluginTypeName = _pluginType.FullName;
			}
			catch (FileNotFoundException ex)
			{
				throw new StructureMapException(102, ex, path.AssemblyName, path.ClassName);
			}
			catch (TypeLoadException ex)
			{
				throw new StructureMapException(103, ex, path.ClassName, path.AssemblyName);
			}

			_defaultKey = defaultKey;
		}

		#endregion

		#region properties

		/// <summary>
		/// The CLR Type that defines the "Plugin" interface for the PluginFamily
		/// </summary>
		public Type PluginType
		{
			get { return _pluginType; }
		}

		/// <summary>
		/// The InstanceKey of the default instance of the PluginFamily
		/// </summary>
		public string DefaultInstanceKey
		{
			get { return _defaultKey; }
			set { _defaultKey = value == null ? string.Empty : value; }
		}

		/// <summary>
		/// Denotes the source or the definition for this Plugin.  Implicit means the
		/// Plugin is defined by a [Pluggable] attribute on the PluggedType.  Explicit
		/// means the Plugin was defined in the StructureMap.config file.
		/// </summary>
		public DefinitionSource DefinitionSource
		{
			get { return _definitionSource; }
			set { _definitionSource = value; }
		}

		/// <summary>
		/// The MementoSource that fetches InstanceMemento definitions for the PluginFamily
		/// </summary>
		public MementoSource Source
		{
			get { return _source; }
			set
			{
				_source = value;

				if (_source != null)
				{
					_source.Family = this;
				}
			}
		}

		public InterceptionChain InterceptionChain
		{
			get { return _interceptionChain; }
			set { _interceptionChain = value; }
		}

		public PluginCollection Plugins
		{
			get { return _plugins; }
		}

		public string PluginTypeName
		{
			get { return _pluginTypeName; }
		}

		public string SourceDescription
		{
			get { return this.Source.Description; }
		}
		#endregion

		/// <summary>
		/// Finds Plugin's that match the PluginType from the assembly and add to the internal
		/// collection of Plugin's 
		/// </summary>
		/// <param name="assembly"></param>
		public void SearchAssemblyGraph(AssemblyGraph assembly)
		{
			Plugin[] plugins = assembly.FindPlugins(this.PluginType);

			foreach (Plugin plugin in plugins)
			{
				_plugins.Add(plugin);
			}
		}


		/// <summary>
		/// Removes any Implicitly defined Plugin and/or Instance from the PluginFamily
		/// </summary>
		public void RemoveImplicitChildren()
		{
			_plugins.RemoveImplicitChildren();
		}

	}
}