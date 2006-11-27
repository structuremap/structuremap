using System;
using System.IO;
using System.Reflection;
using System.Security.Policy;

namespace StructureMap.Graph
{
	/// <summary>
	/// Manages the setup and teardown of a new AppDomain to test a StructureMap configuration.
	/// </summary>
	[Serializable]
	public class RemoteGraphContainer : MarshalByRefObject
	{
		private readonly string _assembliesPath;
		private readonly string _configPath;
		private AppDomain _domain;

		public RemoteGraphContainer(string configPath, string binPath)
		{
			_configPath = Path.GetFullPath(configPath);
			_assembliesPath = Path.GetFullPath(binPath);
		}

		public RemoteGraphContainer(string configPath)
		{
			_configPath = Path.GetFullPath(configPath);
			_assembliesPath = Path.GetDirectoryName(_configPath);
		}


		public string BinPath
		{
			get { return _assembliesPath; }
		}

		public string ConfigPath
		{
			get { return _configPath; }
		}


		public RemoteGraph GetRemoteGraph()
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup = new AppDomainSetup();
			setup.ApplicationBase = _assembliesPath;
			
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			string binPath = Path.Combine(baseDirectory, "bin");
			setup.PrivateBinPath = _assembliesPath + ";" + baseDirectory + ";" + binPath;


			Evidence evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
			string domainName = "StructureMap-" + Guid.NewGuid().ToString();
			_domain = AppDomain.CreateDomain(domainName, evidence, setup);


			// Got to inject this copy of StructureMap.dll into new domain
			string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
			Assembly assem = _domain.Load(assemblyName);
			object obj = _domain.CreateInstanceAndUnwrap(assemblyName, typeof (RemoteGraph).AssemblyQualifiedName);

			RemoteGraph graph = (RemoteGraph) obj;

			graph.Load(_configPath, _assembliesPath);

			return graph;
		}

	}
}