using System;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class InstanceParser : XmlConstants
    {
        private readonly IGraphBuilder _builder;
        private readonly XmlMementoCreator _mementoCreator;

        public InstanceParser(IGraphBuilder builder, XmlMementoCreator mementoCreator)
        {
            _builder = builder;
            _mementoCreator = mementoCreator;
        }

        public void ParseDefaultElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));


            _builder.ConfigureFamily(pluginTypePath, family => {
                InstanceScope scope = findScope(element);
                family.SetScopeTo(scope);

                InstanceMemento memento = _mementoCreator.CreateMemento(element);
                family.AddDefaultMemento(memento);
            });
        }

        public void ParseInstanceElement(XmlElement element)
        {
            var pluginTypePath = new TypePath(element.GetAttribute(PLUGIN_TYPE));

            _builder.ConfigureFamily(pluginTypePath, family => {
                InstanceMemento memento =
                    _mementoCreator.CreateMemento(element);
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