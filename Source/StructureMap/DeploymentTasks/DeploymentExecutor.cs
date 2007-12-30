using System;
using System.Security.Permissions;
using System.Xml;
using StructureMap.Configuration;
using StructureMap.Configuration.Tokens;
using StructureMap.Graph;
using StructureMap.XmlMapping;

namespace StructureMap.DeploymentTasks
{
    [EnvironmentPermission(SecurityAction.Assert, Read="COMPUTERNAME")]
    public class DeploymentExecutor : MarshalByRefObject
    {
        private string _deploymentTarget;
        private MachineSpecificOption _machineOption;
        private string _machineName;
        private string _profileName;
        private string _destinationPath;
        private XmlDocument _sourceConfigDocument;
        private PluginGraphReport _originalGraph;
        private PluginGraphReport _report;
        private InstanceDefaultManager _defaultManager;


        public DeploymentExecutor(
            string configPath,
            string destinationPath,
            string deploymentTarget,
            string profile,
            string machineName,
            MachineSpecificOption machineOption) : base()
        {
            _destinationPath = destinationPath;
            _profileName = profile;
            _machineName = machineName;
            _machineOption = machineOption;
            _deploymentTarget = deploymentTarget;

            initialize(configPath);
        }


        public DeploymentExecutor() : base()
        {
        }

        public void Execute(DeploymentConfiguration configuration)
        {
            _destinationPath = configuration.DestinationPath;
            _profileName = configuration.Profile;
            _machineName = configuration.MachineName;
            _machineOption = configuration.MachineOption;
            _deploymentTarget = configuration.DeploymentTarget;

            string configPath = configuration.ConfigPath;

            initialize(configPath);

            XmlDocument resultConfigDocument = BuildConfigDocument();
            resultConfigDocument.Save(configuration.DestinationPath);
        }

        private void initialize(string configPath)
        {
            _sourceConfigDocument = new XmlDocument();
            _sourceConfigDocument.Load(configPath);

            PluginGraphBuilder builder =
                new PluginGraphBuilder(new ConfigurationParser(_sourceConfigDocument.DocumentElement));
            _report = createPluginGraphReport(builder);
            _defaultManager = builder.DefaultManager;

            // Keep a copy of  original graph
            _originalGraph = createPluginGraphReport(builder);
        }

        private PluginGraphReport createPluginGraphReport(PluginGraphBuilder builder)
        {
            PluginGraph pluginGraph = builder.BuildDiagnosticPluginGraph();

            return builder.Report;
        }


        public XmlDocument BuildConfigDocument()
        {
            ConfigEditor editor = new ConfigEditor(_sourceConfigDocument);

            // filter by deploymet
            // filter by profile


            filterByDeploymentTarget(editor);

            filterByProfileAndMachine(editor);


            return _sourceConfigDocument;
        }

        private void filterByDeploymentTarget(ConfigEditor editor)
        {
            if (_deploymentTarget != string.Empty && _deploymentTarget != null)
            {
                PluginGraphFilter filter = new PluginGraphFilter(_report);
                filter.FilterDeployment(_deploymentTarget);

                foreach (string assemblyName in filter.AssembliesToRemove)
                {
                    editor.RemoveAssembly(assemblyName);
                }

                foreach (string pluginType in filter.FamiliesToRemove)
                {
                    editor.RemovePluginFamily(pluginType);
                }

                foreach (PluginToken plugin in filter.PluginsToRemove)
                {
                    editor.RemovePlugin(plugin.PluginType, plugin.ConcreteKey);
                }
            }
        }

        private void filterByProfileAndMachine(ConfigEditor editor)
        {
            if (_machineOption != MachineSpecificOption.CopyMachineOverrides)
            {
                editor.RemoveAllMachineOptions();
            }

            Profile profile = null;

            if (_machineOption == MachineSpecificOption.UseCurrentMachineOverride)
            {
                profile = _defaultManager.CalculateOverridenDefaults(_machineName, _profileName);
                _defaultManager.ClearMachineOverrides();
            }

            if (_profileName != string.Empty)
            {
                if (profile == null)
                {
                    profile = _defaultManager.GetProfile(_profileName);
                }

                _defaultManager.ClearProfiles();
            }


            if (profile != null)
            {
                editor.CreateDefaultProfile(profile);
            }
        }


        public void Write()
        {
            XmlDocument doc = BuildConfigDocument();
            doc.Save(_destinationPath);
        }
    }
}