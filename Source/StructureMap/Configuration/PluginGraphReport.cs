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
        private Dictionary<Type, FamilyToken> _families = new Dictionary<Type, FamilyToken>();
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

        public FamilyToken FindFamily(string pluginTypeClassName)
        {
            Type type = Type.GetType(pluginTypeClassName);
            if (type != null)
            {
                return FindFamily(type);
            }
            
            foreach (KeyValuePair<Type, FamilyToken> pair in _families)
            {
                if (pair.Value.PluginType.FullName == pluginTypeClassName)
                {
                    return pair.Value;
                }
            }
            
            return null;
        }
        
        public FamilyToken FindFamily(Type pluginType)
        {
            if (!_families.ContainsKey(pluginType))
            {
                throw new MissingPluginFamilyException(pluginType.FullName);
            }

            return _families[pluginType];
        }


        public PluginToken FindPlugin(Type pluginType, string concreteKey)
        {
            return FindFamily(pluginType).FindPlugin(concreteKey);
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

        public TemplateToken FindTemplate(Type pluginType, string templateName)
        {
            FamilyToken family = FindFamily(pluginType);
            return family.FindTemplate(templateName);
        }
    }
}