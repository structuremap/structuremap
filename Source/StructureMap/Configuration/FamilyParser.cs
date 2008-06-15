using System;
using System.Xml;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class FamilyParser : XmlConstants
    {
        private readonly IGraphBuilder _builder;
        private readonly XmlMementoCreator _mementoCreator;

        public FamilyParser(IGraphBuilder builder, XmlMementoCreator mementoCreator)
        {
            _builder = builder;
            _mementoCreator = mementoCreator;
        }

        public void ParseFamily(XmlElement familyElement)
        {
            TypePath typePath = TypePath.CreateFromXmlNode(familyElement);
            _builder.ConfigureFamily(typePath, family =>
            {
                family.DefaultInstanceKey =
                    familyElement.GetAttribute(DEFAULT_KEY_ATTRIBUTE);

                InstanceScope scope = findScope(familyElement);
                family.SetScopeTo(scope);

                attachMementoSource(family, familyElement);
                familyElement.ForEachChild(PLUGIN_NODE).Do(element => attachPlugin(element, family));
                attachInterceptors(family, familyElement);
                attachInstances(family, familyElement, _builder);
            });
        }

        private void attachInstances(PluginFamily family, XmlElement familyElement, IGraphBuilder builder)
        {
            familyElement.ForEachChild(INSTANCE_NODE).Do(element =>
            {
                InstanceMemento memento = _mementoCreator.CreateMemento(element);
                family.AddInstance(memento);
            });
        }

        public void ParseDefaultElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));


            _builder.ConfigureFamily(pluginTypePath, family =>
            {
                InstanceScope scope = findScope(element);
                family.SetScopeTo(scope);

                InstanceMemento memento = _mementoCreator.CreateMemento(element);
                family.AddDefaultMemento(memento);
            });
        }

        public void ParseInstanceElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));

            _builder.ConfigureFamily(pluginTypePath, family =>
            {
                InstanceMemento memento =
                    _mementoCreator.CreateMemento(element);
                family.AddInstance(memento);
            });
        }

        private static InstanceScope findScope(XmlElement familyElement)
        {
            InstanceScope returnValue = InstanceScope.PerRequest;

            familyElement.ForAttributeValue(SCOPE, scope =>
            {
                returnValue = (InstanceScope)Enum.Parse(typeof(InstanceScope), scope);
            });

            return returnValue;
        }

        private void attachMementoSource(PluginFamily family, XmlElement familyElement)
        {
            familyElement.IfHasNode(MEMENTO_SOURCE_NODE).Do(node =>
            {
                InstanceMemento sourceMemento = new XmlAttributeInstanceMemento(node);

                string context = string.Format("MementoSource for {0}\n{1}", TypePath.GetAssemblyQualifiedName(family.PluginType), node.OuterXml);
                _builder.WithSystemObject<MementoSource>(sourceMemento, context,
                                                         source => family.AddMementoSource(source));
            });
        }

        private void attachPlugin(XmlElement pluginElement, PluginFamily family)
        {
            TypePath pluginPath = TypePath.CreateFromXmlNode(pluginElement);
            string concreteKey = pluginElement.GetAttribute(CONCRETE_KEY_ATTRIBUTE);

            string context = "creating a Plugin for " + family.PluginType.AssemblyQualifiedName;
            _builder.WithType(pluginPath, context, pluggedType =>
            {
                Plugin plugin = new Plugin(pluggedType, concreteKey);
                family.AddPlugin(plugin);

                pluginElement.ForTextInChild("Setter/@Name").Do(prop => plugin.Setters.Add(prop));

            });
        }

        private void attachInterceptors(PluginFamily family, XmlElement familyElement)
        {
            string contextBase = string.Format("creating an InstanceInterceptor for {0}\n", TypePath.GetAssemblyQualifiedName(family.PluginType));
            familyElement.ForEachChild("*/Interceptor").Do(element =>
            {
                var interceptorMemento = new XmlAttributeInstanceMemento(element);
                string context = contextBase + element.OuterXml;
                _builder.WithSystemObject<IBuildInterceptor>(interceptorMemento, context,
                                                             interceptor => family.AddInterceptor(interceptor));
            });
        }
    }
}