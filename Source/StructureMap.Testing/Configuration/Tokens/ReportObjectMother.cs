using System;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;

namespace StructureMap.Testing.Configuration.Tokens
{
    public class ReportObjectMother
    {
        private PluginGraphReport _report = new PluginGraphReport();

        public ReportObjectMother()
        {
        }

        public PluginGraphReport Report
        {
            get { return _report; }
        }

        public void AddFamily(Type pluginType)
        {
            FamilyToken family = new FamilyToken(pluginType, "default", new string[0]);
            _report.AddFamily(family);
        }

        public void AddFamily(Type pluginType, string defaultKey)
        {
            FamilyToken family = new FamilyToken(pluginType, defaultKey, new string[0]);
            _report.AddFamily(family);
        }

        public void AddPlugin(Type pluginType, Type pluggedType, string concreteKey)
        {
            PluginToken plugin = new PluginToken(new TypePath(pluggedType), concreteKey, DefinitionSource.Explicit);
            _report.FindFamily(pluginType).AddPlugin(plugin);
        }

        public void AddPrimitiveProperty(Type pluginType, string concreteKey, string propertyName, Type propertyType)
        {
            PropertyDefinition definition =
                new PropertyDefinition(propertyName, propertyType, PropertyDefinitionType.Constructor,
                                       ArgumentType.Primitive);
            PluginToken plugin = _report.FindPlugin(pluginType, concreteKey);

            plugin.AddPropertyDefinition(definition);
        }

        public void AddEnumerationProperty(Type pluginType, string concreteKey, string propertyName, Type propertyType,
                                           string[] enumerationValues)
        {
            PropertyDefinition definition =
                new PropertyDefinition(propertyName, propertyType, PropertyDefinitionType.Constructor,
                                       ArgumentType.Enumeration);
            definition.EnumerationValues = enumerationValues;
            PluginToken plugin = _report.FindPlugin(pluginType, concreteKey);

            plugin.AddPropertyDefinition(definition);
        }

        public void AddChildProperty(Type pluginType, string concreteKey, string propertyName, Type propertyType)
        {
            PropertyDefinition definition =
                new PropertyDefinition(propertyName, propertyType, PropertyDefinitionType.Constructor,
                                       ArgumentType.Child);
            PluginToken plugin = _report.FindPlugin(pluginType, concreteKey);

            plugin.AddPropertyDefinition(definition);
        }


        public void AddChildArrayProperty(Type pluginType, string concreteKey, string propertyName, Type propertyType)
        {
            PropertyDefinition definition =
                new PropertyDefinition(propertyName, propertyType, PropertyDefinitionType.Constructor,
                                       ArgumentType.ChildArray);
            PluginToken plugin = _report.FindPlugin(pluginType, concreteKey);

            plugin.AddPropertyDefinition(definition);
        }
    }
}