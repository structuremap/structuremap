using System.Reflection;
using StructureMap.Source;

namespace StructureMap.Client.Views
{
	public class HTMLSourceMementoSource : SingleEmbeddedXmlMementoSource
	{
		public HTMLSourceMementoSource() : base("View", XmlMementoStyle.AttributeNormalized, Assembly.GetExecutingAssembly(), "StructureMap.Client.Views.ViewDefinition.xml")
		{
		}
	}
}
