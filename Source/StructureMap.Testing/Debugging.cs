using System;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Graph;

namespace StructureMap.Testing
{
	[TestFixture]
	public class Debugging
	{
		[Test, Explicit]
		public void DebugIt()
		{
//			RemoteGraphContainer container = new RemoteGraphContainer(@"C:\Target\StructureMap.config");
//			PluginGraph report = container.GetRemoteGraph().GetPluginGraph();

			PluginGraphBuilder builder = new PluginGraphBuilder("ShareDoc.xml");
			PluginGraphReport report = builder.Report;

			PluginGraph graph = builder.Build();

			InstanceManager manager = new InstanceManager(graph);

			object provider = manager.CreateInstance("StructureMap.DataAccess.IConnectionStringProvider");
		
			Console.WriteLine(provider);
		}


	}
}
