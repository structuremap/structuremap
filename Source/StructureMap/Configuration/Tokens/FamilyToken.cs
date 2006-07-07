using System;
using System.Collections;
using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Configuration.Tokens
{
	[Serializable]
	public class FamilyToken : Deployable
	{
		public static FamilyToken CreateImplicitFamily(PluginFamily family)
		{
			TypePath path = new TypePath(family.PluginType);
			FamilyToken token = new FamilyToken(path, family.DefaultInstanceKey, new string[0]);
			token.DefinitionSource = DefinitionSource.Implicit;

			if (path.CanFindType())
			{
				Type pluginFamilyType = path.FindType();
				PluginFamilyAttribute att = PluginFamilyAttribute.GetAttribute(pluginFamilyType);
				if (att.Scope != InstanceScope.PerRequest)
				{
					token.Scope = att.Scope;
					InterceptorInstanceToken interceptor = new InterceptorInstanceToken(att.Scope);
					token.AddInterceptor(interceptor);
				}
			}

			return token;
		}

		private string _pluginType;
		private string _assemblyName;
		private DefinitionSource _definitionSource = DefinitionSource.Explicit;
		private string _defaultKey;
		private Hashtable _plugins = new Hashtable();
		private InstanceToken _sourceInstance;
		private ArrayList _interceptors = new ArrayList();
		private Hashtable _instances = new Hashtable();
		private Hashtable _templates = new Hashtable();
		private InstanceScope _scope = InstanceScope.PerRequest;

		public FamilyToken() : base()
		{
			
		}

		public FamilyToken(TypePath typePath, string defaultKey, string[] deploymentTargets) : base(deploymentTargets)
		{
			_pluginType = typePath.ClassName;
			_assemblyName = typePath.AssemblyName;
			_defaultKey = defaultKey;
		}

		public string PluginType
		{
			get { return _pluginType; }
			set { _pluginType = value; }
		}

		public string AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
		}

		public DefinitionSource DefinitionSource
		{
			get { return _definitionSource; }
			set { _definitionSource = value; }
		}

		public string DefaultKey
		{
			get { return _defaultKey; }
			set { _defaultKey = value; }
		}

		public InstanceScope Scope
		{
			get { return _scope; }
			set { _scope = value; }
		}

		public InstanceToken SourceInstance
		{
			get { return _sourceInstance; }
			set { _sourceInstance = value; }
		}


		public override string ToString()
		{
			return string.Format("PluginFamily:  {0}, {1} ({2})\nDefaultKey {3}", 
				this.PluginType, 
				this.AssemblyName, 
				this.DefinitionSource, 
				this.DefaultKey);
		}

		public override bool Equals(object obj)
		{
			FamilyToken peer = obj as FamilyToken;
			if (peer == null)
			{
				return false;
			}

			return this.PluginType == peer.PluginType && 
				this.AssemblyName == peer.AssemblyName && 
				this.DefaultKey == peer.DefaultKey && 
				this.DefinitionSource == peer.DefinitionSource;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void MarkTypeCannotBeLoaded(Exception ex)
		{
			Problem problem = new Problem(ConfigurationConstants.COULD_NOT_LOAD_TYPE, ex);
			this.LogProblem(problem);
		}

		public PluginToken[] Plugins
		{
			get
			{
				PluginToken[] returnValue = new PluginToken[_plugins.Count];
				_plugins.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}

		public void AddPlugin(PluginToken plugin)
		{
			plugin.PluginType = this.PluginType;
			_plugins.Add(plugin.ConcreteKey, plugin);
		}

		public PluginToken FindPlugin(string concreteKey)
		{
			return (PluginToken) _plugins[concreteKey];
		}

		public TemplateToken[] Templates
		{
			get
			{
				TemplateToken[] returnValue = new TemplateToken[_templates.Count];
				_templates.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}

		public void AddTemplate(TemplateToken Template)
		{
			Template.PluginType = this.PluginType;
			_templates.Add(Template.TemplateKey, Template);
		}

		public TemplateToken FindTemplate(string templateKey)
		{
			return (TemplateToken) _templates[templateKey];
		}

		public void AddInterceptor(InstanceToken instance)
		{
			_interceptors.Add(instance);
		}

		public InstanceToken[] Interceptors
		{
			get
			{
				return (InstanceToken[]) _interceptors.ToArray(typeof (InstanceToken));
			}
		}


		public void AddInstance(InstanceToken instance)
		{
			_instances.Add(instance.InstanceKey, instance);
		}

		public InstanceToken FindInstance(string instanceKey)
		{
			return (InstanceToken) _instances[instanceKey];
		}

		public InstanceToken[] Instances
		{
			get
			{
				InstanceToken[] returnValue = new InstanceToken[_instances.Count];
				_instances.Values.CopyTo(returnValue, 0);

				return returnValue;
			}
		}

		public void Validate(IInstanceValidator validator)
		{
			foreach (InstanceToken instance in _instances.Values)
			{
				instance.Validate(validator);
			}
		}


		public void ReadInstances(PluginFamily family, PluginGraphReport report)
		{
			string pluginTypeName = family.PluginTypeName;

			try
			{
				InstanceMemento[] mementos = family.Source.GetAllMementos();
				addInstances(mementos, report, pluginTypeName);

				TemplateToken[] tokens = family.Source.GetAllTemplates();
				foreach (TemplateToken templateToken in tokens)
				{
					_templates.Add(templateToken.TemplateKey, templateToken);
				}
			}
			catch (Exception ex)
			{
				Problem problem = new Problem(ConfigurationConstants.MEMENTO_SOURCE_CANNOT_RETRIEVE, ex);
				LogProblem(problem);
			}

			// check if the default instance exists
			checkForDefaultInstanceOfFamily(family);

		}

		private void checkForDefaultInstanceOfFamily(PluginFamily family)
		{
			if (family.DefaultInstanceKey != string.Empty)
			{
				if (!HasInstance(family.DefaultInstanceKey))
				{
					string message = string.Format("Default instance '{0}' of PluginType '{1}' is not configured", 
					                               family.DefaultInstanceKey, this.PluginType);
					Problem problem = new Problem(ConfigurationConstants.CONFIGURED_DEFAULT_KEY_CANNOT_BE_FOUND, message);
					LogProblem(problem);

					family.DefaultInstanceKey = string.Empty;
				}
			}
		}

		private void addInstances(InstanceMemento[] mementos, PluginGraphReport report, string pluginTypeName)
		{
			foreach (InstanceMemento memento in mementos)
			{
				InstanceToken instance = new InstanceToken(pluginTypeName, report, memento);
				instance.Source = memento.DefinitionSource;
				AddInstance(instance);
			}
		}

		public override GraphObject[] Children
		{
			get
			{
				ArrayList list = new ArrayList();
				list.AddRange(this.Plugins);
				list.AddRange(this.Interceptors);
				list.AddRange(this.Instances);
				list.AddRange(this.Templates);

				if (_sourceInstance != null)
				{
					list.Add(_sourceInstance);
				}

				list.Sort();

				return (GraphObject[]) list.ToArray(typeof (GraphObject));
			}
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandleFamily(this);
		}

		protected override string key
		{
			get { return this.PluginType; }
		}

		public bool HasInstance(string instanceKey)
		{
			return _instances.ContainsKey(instanceKey);
		}

		public bool HasPlugin(string concreteKey)
		{
			return _plugins.ContainsKey(concreteKey);
		}

		public void FilterInstances(string defaultKey)
		{
			InstanceToken instance = (InstanceToken) _instances[defaultKey];
			_instances = new Hashtable();
			_instances.Add(instance.InstanceKey, instance);
		}
	}
}
