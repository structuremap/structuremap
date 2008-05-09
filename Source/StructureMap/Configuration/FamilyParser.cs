using System;
using System.Xml;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
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

        public void ParseFamily(XmlElement familyElement)
        {
            TypePath typePath = TypePath.CreateFromXmlNode(familyElement);
            _builder.ConfigureFamily(typePath, delegate(PluginFamily family)
                                                   {
                                                       family.DefaultInstanceKey =
                                                           familyElement.GetAttribute(XmlConstants.DEFAULT_KEY_ATTRIBUTE);

                                                       InstanceScope scope = findScope(familyElement);
                                                       family.SetScopeTo(scope);

                                                       attachMementoSource(family, familyElement);
                                                       attachPlugins(family, familyElement);
                                                       attachInterceptors(family, familyElement);
                                                   });
        }

        public void ParseDefaultElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(XmlConstants.PLUGIN_TYPE));


            _builder.ConfigureFamily(pluginTypePath,
                                     delegate(PluginFamily family)
                                         {
                                             // TODO:  there's a little duplication here
                                             InstanceScope scope = findScope(element);
                                             family.SetScopeTo(scope);

                                             Type pluginType = family.PluginType;

                                             string name = element.GetAttribute(XmlConstants.NAME);
                                             if (string.IsNullOrEmpty(name))
                                             {
                                                 name = "DefaultInstanceOf" + pluginTypePath.AssemblyQualifiedName;
                                             }

                                             InstanceMemento memento = _mementoCreator.CreateMemento(element);
                                             memento.InstanceKey = name;

                                             family.DefaultInstanceKey = name;

                                             family.AddInstance(memento);
                                         });
        }

        public void ParseInstanceElement(XmlElement element)
        {
            TypePath pluginTypePath = new TypePath(element.GetAttribute(XmlConstants.PLUGIN_TYPE));
            
            _builder.ConfigureFamily(pluginTypePath, delegate(PluginFamily family)
                                                         {
                                                             InstanceMemento memento = _mementoCreator.CreateMemento(element);
                                                             family.AddInstance(memento);
                                                         });
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
        private void attachMementoSource(PluginFamily family, XmlElement familyElement)
        {
            XmlNode sourceNode = familyElement[XmlConstants.MEMENTO_SOURCE_NODE];
            if (sourceNode != null)
            {
                InstanceMemento sourceMemento = new XmlAttributeInstanceMemento(sourceNode);

                string context = "MementoSource for " + TypePath.GetAssemblyQualifiedName(family.PluginType);
                _builder.WithSystemObject<MementoSource>(sourceMemento, context, delegate (MementoSource source)
                                                                                     {
                                                                                         family.AddMementoSource(source);
                                                                                     });


            }
        }

        private void attachPlugins(PluginFamily family, XmlElement familyElement)
        {
            // TODO:  3.5 lambda cleanup
            XmlNodeList pluginNodes = familyElement.SelectNodes(XmlConstants.PLUGIN_NODE);
            foreach (XmlElement pluginElement in pluginNodes)
            {
                TypePath pluginPath = TypePath.CreateFromXmlNode(pluginElement);
                string concreteKey = pluginElement.GetAttribute(XmlConstants.CONCRETE_KEY_ATTRIBUTE);

                string context = "creating a Plugin for " + family.PluginType.AssemblyQualifiedName;
                _builder.WithType(pluginPath, context, delegate(Type pluggedType)
                                                           {
                                                               Plugin plugin = new Plugin(pluggedType, concreteKey);
                                                               family.Plugins.Add(plugin);

                                                               foreach (XmlElement setterElement in pluginElement.ChildNodes)
                                                               {
                                                                   string setterName = setterElement.GetAttribute("Name");
                                                                   plugin.Setters.Add(setterName);
                                                               }
                                                           });


            }
        }

        // TODO:  3.5 lambda cleanup
        private void attachInterceptors(PluginFamily family, XmlElement familyElement)
        {
            XmlNode interceptorChainNode = familyElement[XmlConstants.INTERCEPTORS_NODE];
            if (interceptorChainNode == null)
            {
                return;
            }

            string context = "Creating an InstanceInterceptor for " + TypePath.GetAssemblyQualifiedName(family.PluginType);
            foreach (XmlNode interceptorNode in interceptorChainNode.ChildNodes)
            {
                XmlAttributeInstanceMemento interceptorMemento = new XmlAttributeInstanceMemento(interceptorNode);


                _builder.WithSystemObject<IBuildInterceptor>(interceptorMemento, context, delegate(IBuildInterceptor interceptor)
                                                                                                 {
                                                                                                     family.AddInterceptor(interceptor);
                                                                                                 });
                
            }
        }
    }
}