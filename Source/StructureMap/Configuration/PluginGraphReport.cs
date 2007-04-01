using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Configuration.Tokens;
using StructureMap.Exceptions;
using StructureMap.Graph;

namespace StructureMap.Configuration
{
    [Serializable]
    public class PluginGraphReport : GraphObject
    {
        private Hashtable _assemblies = new Hashtable();
        private Dictionary<TypePath, FamilyToken> _families = new Dictionary<TypePath, FamilyToken>();
        private InstanceDefaultManager _defaultManager;

        public PluginGraphReport()
        {
        }

        public PluginGraphReport(PluginGraph pluginGraph)
        {
            ReadFromPluginGraph(pluginGraph);
        }

        public void ReadFromPluginGraph(PluginGraph pluginGraph)
        {
            ImportImplicitChildren(pluginGraph);
            AnalyzeInstances(pluginGraph);

            Profile defaultProfile = pluginGraph.DefaultManager.CalculateDefaults();

            InstanceManager manager = new InstanceManager();
            try
            {
                manager = new InstanceManager(pluginGraph);
            }
            catch (Exception ex)
            {
                Problem problem = new Problem(ConfigurationConstants.FATAL_ERROR, ex);
                LogProblem(problem);
            }

            IInstanceValidator validator = new InstanceValidator(pluginGraph, defaultProfile, manager);
            ValidateInstances(validator);
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
            _families.Add(family.TypePath, family);
        }

        public FamilyToken FindFamily(string pluginTypeClassName)
        {
            TypePath path = TypePath.GetTypePath(pluginTypeClassName);
            if (path != null)
            {
                return FindFamily(path);
            }

            foreach (KeyValuePair<TypePath, FamilyToken> pair in _families)
            {
                if (pair.Key.Matches(pluginTypeClassName))
                {
                    return pair.Value;
                }
            }

            return null;
        }

        public bool HasFamily(Type pluginType)
        {
            return _families.ContainsKey(new TypePath(pluginType));
        }

        public FamilyToken FindFamily(Type pluginType)
        {
            TypePath path = new TypePath(pluginType);

            if (!_families.ContainsKey(path))
            {
                throw new MissingPluginFamilyException(path.AssemblyQualifiedName);
            }

            return _families[path];
        }


        public PluginToken FindPlugin(TypePath pluginTypePath, string concreteKey)
        {
            return FindFamily(pluginTypePath).FindPlugin(concreteKey);
        }

        public PluginToken FindPlugin(Type pluginType, string concreteKey)
        {
            return FindPlugin(new TypePath(pluginType), concreteKey);
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
            FamilyToken familyToken = FindFamily(family.PluginType);

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
                FamilyToken token = FindFamily(family.PluginType);
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

        public TemplateToken FindTemplate(TypePath pluginTypePath, string templateName)
        {
            FamilyToken family = FindFamily(pluginTypePath);
            return family.FindTemplate(templateName);
        }

        public FamilyToken FindFamily(TypePath path)
        {
            return _families[path];
        }
    }
}