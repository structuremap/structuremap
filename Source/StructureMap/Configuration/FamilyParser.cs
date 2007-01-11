using System;
using System.Xml;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;
using StructureMap.Source;

namespace StructureMap.Configuration
{
	public class FamilyParser
	{
		private readonly IGraphBuilder _builder;

		public FamilyParser(IGraphBuilder builder)
		{
			_builder = builder;
		}

		public void ParseFamily(XmlElement familyElement)
		{
			TypePath typePath = TypePath.CreateFromXmlNode(familyElement);
			string defaultKey = familyElement.GetAttribute(XmlConstants.DEFAULT_KEY_ATTRIBUTE);
			string[] deploymentTargets = ConfigurationParser.GetDeploymentTargets(familyElement);

			
			InstanceScope scope = findScope(familyElement);

			_builder.AddPluginFamily(typePath, defaultKey, deploymentTargets, scope);

			attachMementoSource(familyElement, typePath);
            attachPlugins(typePath, familyElement);
            attachInterceptors(typePath, familyElement);
		}

		private InstanceScope findScope(XmlElement familyElement)
		{
			InstanceScope returnValue = InstanceScope.PerRequest;

			string scopeString = familyElement.GetAttribute(XmlConstants.SCOPE_ATTRIBUTE);
			if (scopeString != null && scopeString != string.Empty)
			{
				returnValue = (InstanceScope) Enum.Parse(typeof(InstanceScope), scopeString);
			}

			return returnValue;
		}

		private void attachMementoSource(XmlElement familyElement, TypePath pluginTypePath)
		{
			XmlNode sourceNode = familyElement[XmlConstants.MEMENTO_SOURCE_NODE];
			if (sourceNode != null)
			{
				InstanceMemento sourceMemento = new XmlAttributeInstanceMemento(sourceNode);
                _builder.AttachSource(pluginTypePath, sourceMemento);
			}
		}

		private void attachPlugins(TypePath pluginTypePath, XmlElement familyElement)
		{
			XmlNodeList pluginNodes = familyElement.SelectNodes(XmlConstants.PLUGIN_NODE);
			foreach (XmlElement pluginElement in pluginNodes)
			{
				TypePath pluginPath = TypePath.CreateFromXmlNode(pluginElement);
				string concreteKey = pluginElement.GetAttribute(XmlConstants.CONCRETE_KEY_ATTRIBUTE);

				_builder.AddPlugin(pluginTypePath, pluginPath, concreteKey);

				foreach (XmlElement setterElement in pluginElement.ChildNodes)
				{
					string setterName = setterElement.GetAttribute("Name");
					_builder.AddSetter(pluginTypePath, concreteKey, setterName);
				}
			}
		}

		private void attachInterceptors(TypePath pluginTypePath, XmlElement familyElement)
		{
			XmlNode interceptorChainNode = familyElement[XmlConstants.INTERCEPTORS_NODE];
			if (interceptorChainNode == null)
			{
				return;
			}

			foreach (XmlNode interceptorNode in interceptorChainNode.ChildNodes)
			{
				XmlAttributeInstanceMemento interceptorMemento = new XmlAttributeInstanceMemento(interceptorNode);
				_builder.AddInterceptor(pluginTypePath, interceptorMemento);
			}
		}

	}
}
