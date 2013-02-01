using System;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Util;

namespace StructureMap.Configuration
{
    public class InstanceParser : XmlConstants, IPluginFactory
    {
        private readonly Cache<string, Type> _aliases = new Cache<string, Type>(); 
        private readonly IGraphBuilder _builder;

        public InstanceParser(IGraphBuilder builder)
        {
            _builder = builder;
        }

        public Plugin PluginFor(Type pluginType, string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            if (name.Contains(","))
            {
                var pluggedType = new TypePath(name).FindType();
                return PluginCache.GetPlugin(pluggedType);
            }

            return _aliases.Has(name) ? PluginCache.GetPlugin(_aliases[name]) : null;
        }

        public void ParseAlias(XmlElement element)
        {
            var attribute = element.GetAttribute("Key");
            var typeName = element.GetAttribute("Type");

            if (attribute.IsEmpty() || typeName.IsEmpty())
            {
                throw new MalformedAliasException(element.OuterXml);
            }

            _aliases[attribute] = new TypePath(typeName).FindType();
        }

        public void ParseDefaultElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));


            _builder.ConfigureFamily(pluginTypePath, family => {
                InstanceScope scope = findScope(element);
                family.SetScopeTo(scope);

                InstanceMemento memento = ConfigurationParser.CreateMemento(element);
                family.AddDefaultMemento(memento);
            });
        }

        public void ParseInstanceElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));

            _builder.ConfigureFamily(pluginTypePath, family => {
                InstanceMemento memento =
                    ConfigurationParser.CreateMemento(element);
                family.AddInstance(memento);
            });
        }

        private static InstanceScope findScope(XmlElement familyElement)
        {
            var returnValue = InstanceScope.PerRequest;

            familyElement.ForAttributeValue(SCOPE,
                                            scope => { returnValue = (InstanceScope) Enum.Parse(typeof (InstanceScope), scope); });

            return returnValue;
        }
    }
}