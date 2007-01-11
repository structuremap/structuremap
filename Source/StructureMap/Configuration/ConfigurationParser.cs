using System;
using System.Collections;
using System.IO;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Graph.Configuration;
using StructureMap.Source;

namespace StructureMap.Configuration
{
	public class ConfigurationParser
	{
		#region statics
		public static ConfigurationParser[] GetParsers(XmlDocument document, string includePath)
		{
			string folder = Path.GetDirectoryName(includePath);

			ArrayList list = new ArrayList();
			list.Add(new ConfigurationParser(document.DocumentElement));

			string includedPath = null;

			try
			{
				XmlNodeList includeNodes = document.DocumentElement.SelectNodes(XmlConstants.INCLUDE_NODE);
				foreach (XmlElement includeElement in includeNodes)
				{
					XmlDocument includedDoc = new XmlDocument();
					string fileName = includeElement.GetAttribute("File");

					if (fileName == string.Empty)
					{
						throw new ApplicationException("The File attribute on the Include node is required");
					}

					try
					{
						includedPath = Path.Combine(folder, fileName);
						includedDoc.Load(includedPath);

						ConfigurationParser parser = new ConfigurationParser(includedDoc.DocumentElement);
						list.Add(parser);
					}
					catch (Exception ex)
					{
						throw new StructureMapException(150, ex, fileName);
					}
				}
			}
			catch (Exception ex)
			{
				throw new StructureMapException(100, includedPath, ex);
			}

			return (ConfigurationParser[]) list.ToArray(typeof (ConfigurationParser));
		}

		public static string[] GetDeploymentTargets(XmlNode node)
		{
			string[] returnValue = new string[0];

			XmlAttribute att = node.Attributes[XmlConstants.DEPLOYMENT_ATTRIBUTE];
			if (att != null)
			{
				string deployTargetArray = att.Value;

				deployTargetArray = deployTargetArray.Replace(" ,", ",");
				deployTargetArray = deployTargetArray.Replace(", ", ",");


				returnValue = deployTargetArray.Split(',');
			}

			return returnValue;
		}
		#endregion

		private readonly XmlNode _structureMapNode;
		private IGraphBuilder _builder;

		public ConfigurationParser(XmlNode structureMapNode)
		{
			_structureMapNode = structureMapNode;
		}

		public void ParseAssemblies(IGraphBuilder builder)
		{
			_builder = builder;
			parseAssemblies();
		}

		public void ParseFamilies(IGraphBuilder builder)
		{
			_builder = builder;
			parseFamilies();
		}

		public void ParseInstances(IGraphBuilder builder)
		{
			_builder = builder;
			parseInstances();

			XmlNodeList familyNodes = findNodes(XmlConstants.PLUGIN_FAMILY_NODE);
			foreach (XmlElement familyElement in familyNodes)
			{
			    TypePath typePath = TypePath.CreateFromXmlNode(familyElement);
				this.attachInstances(typePath, familyElement);
			}
		}

		private void parseAssemblies()
		{
			XmlNodeList assemblyNodes = findNodes(XmlConstants.ASSEMBLY);
			foreach (XmlNode assemblyNode in assemblyNodes)
			{
				string assemblyName = assemblyNode.Attributes[XmlConstants.NAME].Value;
				string[] deploymentTargets = GetDeploymentTargets(assemblyNode);

				_builder.AddAssembly(assemblyName, deploymentTargets);
			}
		}

		private XmlNodeList findNodes(string nodeName)
		{
			return _structureMapNode.SelectNodes(nodeName);
		}




		private void parseFamilies()
		{
			FamilyParser familyParser = new FamilyParser(_builder);
			XmlNodeList familyNodes = findNodes(XmlConstants.PLUGIN_FAMILY_NODE);
			foreach (XmlElement familyElement in familyNodes)
			{
				familyParser.ParseFamily(familyElement);
			}			
		}




		private void attachInstances(TypePath pluginTypePath, XmlElement familyElement)
		{
			foreach (XmlNode instanceNode in familyElement.ChildNodes)
			{
				if (instanceNode.Name != XmlConstants.INSTANCE_NODE)
				{
					continue;
				}

				XmlNodeInstanceMemento memento = new XmlNodeInstanceMemento(instanceNode, XmlConstants.TYPE_ATTRIBUTE, XmlConstants.KEY_ATTRIBUTE);
				_builder.RegisterMemento(pluginTypePath, memento);
			}
		}

		private void parseInstances()
		{
			XmlNode instancesNode = _structureMapNode[XmlConstants.INSTANCES_NODE];
			if (instancesNode == null)
			{
				return;
			}

			foreach (XmlNode instanceNode in instancesNode)
			{
				string pluginTypeName = instanceNode.Name;
			    TypePath typePath = TypePath.TypePathForFullName(pluginTypeName);

				XmlNodeInstanceMemento memento = new XmlNodeInstanceMemento(instanceNode, XmlConstants.TYPE_ATTRIBUTE, XmlConstants.KEY_ATTRIBUTE);

				_builder.RegisterMemento(typePath, memento);
			}
		}
	}
}
