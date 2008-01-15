using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;

namespace StructureMap.Configuration.Tokens
{
    [Serializable]
    public class FamilyToken : Deployable
    {
        private string _defaultKey;
        private DefinitionSource _definitionSource = DefinitionSource.Explicit;
        private Hashtable _instances = new Hashtable();
        private ArrayList _interceptors = new ArrayList();
        private Dictionary<string, PluginToken> _plugins = new Dictionary<string, PluginToken>();
        private InstanceScope _scope = InstanceScope.PerRequest;
        private InstanceToken _sourceInstance;
        private Hashtable _templates = new Hashtable();
        private TypePath _typePath;

        public FamilyToken() : base()
        {
        }

        public FamilyToken(Type type, string defaultKey, string[] deploymentTargets)
            : this(new TypePath(type), defaultKey, deploymentTargets)
        {
        }

        public FamilyToken(TypePath path, string defaultKey, string[] deploymentTargets) : base(deploymentTargets)
        {
            _typePath = path;
            _defaultKey = defaultKey;
        }

        public string PluginTypeName
        {
            get { return _typePath.AssemblyQualifiedName; }
        }

        public string FullPluginTypeName
        {
            get { return _typePath.ClassName; }
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

        public PluginToken[] Plugins
        {
            get
            {
                PluginToken[] returnValue = new PluginToken[_plugins.Count];
                _plugins.Values.CopyTo(returnValue, 0);

                return returnValue;
            }
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

        public InstanceToken[] Interceptors
        {
            get { return (InstanceToken[]) _interceptors.ToArray(typeof (InstanceToken)); }
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

        public override GraphObject[] Children
        {
            get
            {
                ArrayList list = new ArrayList();
                list.AddRange(Plugins);
                list.AddRange(Interceptors);
                list.AddRange(Instances);
                list.AddRange(Templates);

                if (_sourceInstance != null)
                {
                    list.Add(_sourceInstance);
                }

                list.Sort();

                return (GraphObject[]) list.ToArray(typeof (GraphObject));
            }
        }

        protected override string key
        {
            get { return PluginTypeName; }
        }

        public TypePath TypePath
        {
            get { return _typePath; }
        }

        public string AssemblyName
        {
            get { return _typePath.AssemblyName; }
        }

        public static FamilyToken CreateImplicitFamily(PluginFamily family)
        {
            FamilyToken token =
                new FamilyToken(new TypePath(family.PluginType), family.DefaultInstanceKey, new string[0]);
            token.DefinitionSource = DefinitionSource.Implicit;


            PluginFamilyAttribute att = PluginFamilyAttribute.GetAttribute(family.PluginType);
            if (att != null)
            {
                if (att.Scope != InstanceScope.PerRequest)
                {
                    token.Scope = att.Scope;
                    InterceptorInstanceToken interceptor = new InterceptorInstanceToken(att.Scope);
                    token.AddInterceptor(interceptor);
                }
            }

            return token;
        }


        public override string ToString()
        {
            return string.Format("PluginFamily:  {0}, {1} ({2})\nDefaultKey {3}",
                                 PluginTypeName,
                                 AssemblyName,
                                 DefinitionSource,
                                 DefaultKey);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            FamilyToken peer = obj as FamilyToken;
            if (peer == null)
            {
                return false;
            }

            return PluginTypeName == peer.PluginTypeName &&
                   AssemblyName == peer.AssemblyName &&
                   DefaultKey == peer.DefaultKey &&
                   DefinitionSource == peer.DefinitionSource;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }


        public void AddPlugin(PluginToken plugin)
        {
            plugin.PluginType = _typePath;
            _plugins.Add(plugin.ConcreteKey, plugin);
        }

        public PluginToken FindPlugin(string concreteKey)
        {
            if (_plugins.ContainsKey(concreteKey))
            {
                return _plugins[concreteKey];
            }

            return null;
        }

        public void AddTemplate(TemplateToken Template)
        {
            Template.PluginType = PluginTypeName;
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


        public void AddInstance(InstanceToken instance)
        {
            if (_instances.ContainsKey(instance.InstanceKey))
            {
                string message =
                    string.Format("Duplicate Instance '{0}' of PluginFamily '{1}'", instance.InstanceKey,
                                  _typePath.AssemblyQualifiedName);
                throw new ApplicationException(message);
            }

            _instances.Add(instance.InstanceKey, instance);
        }

        public InstanceToken FindInstance(string instanceKey)
        {
            return (InstanceToken) _instances[instanceKey];
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
            try
            {
                InstanceMemento[] mementos = family.Source.GetAllMementos();
                addInstances(mementos, report, family.PluginType);

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
                                                   family.DefaultInstanceKey, PluginTypeName);
                    Problem problem =
                        new Problem(ConfigurationConstants.CONFIGURED_DEFAULT_KEY_CANNOT_BE_FOUND, message);
                    LogProblem(problem);

                    family.DefaultInstanceKey = string.Empty;
                }
            }
        }

        private void addInstances(InstanceMemento[] mementos, PluginGraphReport report, Type pluginType)
        {
            foreach (InstanceMemento memento in mementos)
            {
                InstanceToken instance = new InstanceToken(pluginType, report, memento);
                instance.Source = memento.DefinitionSource;
                AddInstance(instance);
            }
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleFamily(this);
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

        public void MarkAsInvalidType(Exception ex)
        {
            Problem problem = new Problem(ConfigurationConstants.COULD_NOT_LOAD_TYPE, ex);
            LogProblem(problem);
        }
    }
}