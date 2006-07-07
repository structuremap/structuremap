using StructureMap.Client.Views;
using StructureMap.Configuration;

namespace StructureMap.Client.Controllers
{
	/// <summary>
	/// Directs StructureMap to use an instance of HTMLSourceMementoSource for the 
	/// MementoSource.  Note that the MementoSource specified has to have a
	/// no argument constructor
	/// </summary>
	[PluginFamily(SourceType = typeof(HTMLSourceMementoSource))]
	public interface IHTMLSource
	{
		string BuildHTML(GraphObject subject);
	}
}
