using System;
using System.Xml;

namespace StructureMap.Source
{
	[Obsolete("No longer used within core of StructureMap")]
	public class EmbeddedMementoSource : BasicXmlMementoSource
	{
		public EmbeddedMementoSource(XmlNode Node)
			: base(Node, "Instance")
		{
		}

		public override MementoSourceType SourceType
		{
			get { return MementoSourceType.Embedded; }
		}

		public override string Description
		{
			get { return "EmbeddedMementoSource"; }
		}

	}
}