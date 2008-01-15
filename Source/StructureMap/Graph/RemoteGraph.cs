using System;
using System.Reflection;
using StructureMap.Configuration;
using StructureMap.DeploymentTasks;

namespace StructureMap.Graph
{
    /// <summary>
    /// Manages the setup and teardown of a new AppDomain to test a StructureMap configuration.
    /// </summary>
    public class RemoteGraph : MarshalByRefObject
    {
        private string _binPath;
        private string _configPath;
        private PluginGraph _pluginGraph;
        private PluginGraphReport _report;

        public RemoteGraph() : base()
        {
        }

        public string ConfigPath
        {
            get { return _configPath; }
        }


        public void Load(string configPath, string binPath)
        {
            _configPath = configPath;
            _binPath = binPath;
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private void initializeChildren()
        {
            if (_pluginGraph != null)
            {
                return;
            }

            PluginGraphBuilder builder = new PluginGraphBuilder(_configPath);
            _pluginGraph = builder.BuildDiagnosticPluginGraph();
            _report = builder.Report;
        }


        public DeploymentExecutor CreateDeploymentExecutor()
        {
            return new DeploymentExecutor();
        }

        public PluginGraphReport GetReport()
        {
            initializeChildren();
            return _report;
        }

        public PluginGraph GetPluginGraph()
        {
            PluginGraphBuilder builder = new PluginGraphBuilder(_configPath);
            return builder.Build();
        }


        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string fileName = string.Format("{0}\\{1}.Dll", _binPath, args.Name);
            return Assembly.LoadFile(fileName);
        }
    }
}