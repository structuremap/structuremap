using System;
using System.Xml;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class FamilyParser
    {
        private readonly IGraphBuilder _builder;
        private readonly XmlMementoCreator _mementoCreator;

        public FamilyParser(IGraphBuilder builder, XmlMementoCreator mementoCreator)
        {
            _builder = builder;
            _mementoCreator = mementoCreator;
        }

        // TODO:  Standard way in this class to get a PluginType, Maybe more into IGraphBuilder
        public void ParseFamily(XmlElement familyElement)
        {
            TypePath typePath = TypePath.CreateFromXmlNode(familyElement);

            // TODO:  throw error if PluginType cannot be found.  Right here!
            Type pluginType;
            try
            {
                pluginType = typePath.FindType();
            }
            catch (Exception ex)
            {
                // TODO:  put error in PluginGraph
                throw new StructureMapException(103, ex, typePath.ClassName, typePath.AssemblyName);
            }


            string defaultKey = familyElement.GetAttribute(XmlConstants.DEFAULT_KEY_ATTRIBUTE);

            InstanceScope scope = findScope(familyElement);

            _builder.AddPluginFamily(pluginType, defaultKey, scope);

            attachMementoSource(pluginType, familyElement);
            attachPlugins(pluginType, familyElement);
            attachInterceptors(pluginType, familyElement);
        }

        public void ParseDefaultElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(XmlConstants.PLUGIN_TYPE));
            // TODO:  Gotta throw exception if the type cannot be found

            Type pluginType = pluginTypePath.FindType();


            InstanceScope scope = findScope(element);
            string name = element.GetAttribute(XmlConstants.NAME);
            if (string.IsNullOrEmpty(name))
            {
                name = "DefaultInstanceOf" + pluginTypePath.AssemblyQualifiedName;
            }

            InstanceMemento memento = _mementoCreator.CreateMemento(element);
            memento.InstanceKey = name;

            _builder.AddPluginFamily(pluginType, name, scope);
            _builder.RegisterMemento(pluginType, memento);
        }

        public void ParseInstanceElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(XmlConstants.PLUGIN_TYPE));
            // TODO:  gotta throw if type cannot be found
            Type pluginType = pluginTypePath.FindType();

            InstanceScope scope = findScope(element);

            InstanceMemento memento = _mementoCreator.CreateMemento(element);

            _builder.RegisterMemento(pluginType, memento);
        }

        private InstanceScope findScope(XmlElement familyElement)
        {
            InstanceScope returnValue = InstanceScope.PerRequest;

            string scopeString = familyElement.GetAttribute(XmlConstants.SCOPE_ATTRIBUTE);
            if (scopeString != null && scopeString != string.Empty)
            {
                returnValue = (InstanceScope) Enum.Parse(typeof (InstanceScope), scopeString);
            }

            return returnValue;
        }

        // TODO:  change to many
        private void attachMementoSource(Type pluginType, XmlElement familyElement)
        {
            XmlNode sourceNode = familyElement[XmlConstants.MEMENTO_SOURCE_NODE];
            if (sourceNode != null)
            {
                InstanceMemento sourceMemento = new XmlAttributeInstanceMemento(sourceNode);
                _builder.AttachSource(pluginType, sourceMemento);
            }
        }

        private void attachPlugins(Type pluginType, XmlElement familyElement)
        {
            // TODO:  3.5 lambda cleanup
            XmlNodeList pluginNodes = familyElement.SelectNodes(XmlConstants.PLUGIN_NODE);
            foreach (XmlElement pluginElement in pluginNodes)
            {
                TypePath pluginPath = TypePath.CreateFromXmlNode(pluginElement);
                string concreteKey = pluginElement.GetAttribute(XmlConstants.CONCRETE_KEY_ATTRIBUTE);

                _builder.AddPlugin(pluginType, pluginPath, concreteKey);

                foreach (XmlElement setterElement in pluginElement.ChildNodes)
                {
                    string setterName = setterElement.GetAttribute("Name");
                    _builder.AddSetter(pluginType, concreteKey, setterName);
                }
            }
        }

        // TODO:  3.5 lambda cleanup
        private void attachInterceptors(Type pluginType, XmlElement familyElement)
        {
            XmlNode interceptorChainNode = familyElement[XmlConstants.INTERCEPTORS_NODE];
            if (interceptorChainNode == null)
            {
                return;
            }

            foreach (XmlNode interceptorNode in interceptorChainNode.ChildNodes)
            {
                XmlAttributeInstanceMemento interceptorMemento = new XmlAttributeInstanceMemento(interceptorNode);
                _builder.AddInterceptor(pluginType, interceptorMemento);
            }
        }
    }
}