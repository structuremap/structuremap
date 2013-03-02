using System;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Configuration.Xml
{
    public class InstanceParser : XmlConstants, IPluginFactory
    {
        private readonly Cache<string, Type> _aliases = new Cache<string, Type>(); 
        private readonly IGraphBuilder _builder;

        public InstanceParser(IGraphBuilder builder)
        {
            _builder = builder;
        }

        [Obsolete("Think this should just return PluggedType")]
        public Plugin PluginFor(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            if (name.Contains(","))
            {
                var pluggedType = new TypePath(name).FindType();
                return new Plugin(pluggedType);
            }

            return _aliases.Has(name) ?  new Plugin(_aliases[name]) : null;
        }

        public void ParseAlias(XmlElement element)
        {
            var attribute = element.GetAttribute("Key");
            var typeName = element.GetAttribute("Type");

            if (String.IsNullOrEmpty(attribute) || String.IsNullOrEmpty(typeName))
            {
                throw new MalformedAliasException(element.OuterXml);
            }

            _aliases[attribute] = new TypePath(typeName).FindType();
        }

        public void ParseDefaultElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));


            _builder.ConfigureFamily(pluginTypePath, family => {
                var scope = findScope(element);
                family.SetScopeTo(scope);

                var memento = ConfigurationParser.CreateMemento(element);
                var instance = memento.ToInstance(this, family.PluginType);
                family.SetDefault(instance);
            });
        }

        public void ParseInstanceElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));

            _builder.ConfigureFamily(pluginTypePath, family => {
                InstanceMemento memento = ConfigurationParser.CreateMemento(element);
                var instance = memento.ToInstance(this, family.PluginType);

                family.AddInstance(instance);
            });
        }

        private static ILifecycle findScope(XmlElement familyElement)
        {
            var returnValue = Lifecycles.Transient;

            //familyElement.ForAttributeValue(SCOPE,
            //                                scope => { returnValue = scope; });

            return returnValue;
        }
    }
}