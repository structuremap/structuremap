using System;
using System.Xml;

namespace StructureMap.Source
{
	/// <summary>
	/// MementoSource that reads InstanceMemento's from the &lt;Instances&gt; node
	/// in the StructureMap.config file
	/// </summary>
	[Obsolete("No longer used")]
	public class ConfigInstanceMementoSource : XmlMementoSource
	{
		private XmlNode _root;

		public ConfigInstanceMementoSource(Type pluginType)
			: base(pluginType.FullName, "Type", "Key", XmlMementoStyle.NodeNormalized)
		{
			initializeToDefaultConfigFile();
		}

		private void initializeToDefaultConfigFile()
		{
			XmlDocument doc = new XmlDocument();
			string fileName = PluginGraphBuilder.GetStructureMapConfigurationPath();
			doc.Load(fileName);
	
			_root = doc.DocumentElement.SelectSingleNode("Instances");
		}

		public ConfigInstanceMementoSource(Type pluginType, XmlNode StructureMapNode)
			: base(pluginType.FullName, "Type", "Key", XmlMementoStyle.NodeNormalized)
		{
			if (StructureMapNode == null)
			{
				initializeToDefaultConfigFile();
			}
			else
			{
				_root = StructureMapNode.SelectSingleNode("Instances");	
			}
		}


		protected override XmlNode getRootNode()
		{
			return _root;
		}

		public override MementoSourceType SourceType
		{
			get { return MementoSourceType.ConfigInstance; }
		}

		public override string Description
		{
			get { return "ConfigInstanceMementoSource"; }
		}


	}
}