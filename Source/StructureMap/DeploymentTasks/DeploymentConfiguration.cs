using System;

namespace StructureMap.DeploymentTasks
{
	[Serializable]
	public class DeploymentConfiguration
	{
		private string _configPath = string.Empty;
		private string _destinationPath = string.Empty;
		private string _deploymentTarget = string.Empty;
		private string _profile = string.Empty;
		private MachineSpecificOption _machineOption = MachineSpecificOption.CopyMachineOverrides;
		private string _machineName = string.Empty;

		public DeploymentConfiguration()
		{
		}

		public string ConfigPath
		{
			get { return _configPath; }
			set { _configPath = value; }
		}

		public string DestinationPath
		{
			get { return _destinationPath; }
			set { _destinationPath = value; }
		}

		public string DeploymentTarget
		{
			get { return _deploymentTarget; }
			set { _deploymentTarget = value; }
		}

		public string Profile
		{
			get { return _profile; }
			set { _profile = value; }
		}

		public MachineSpecificOption MachineOption
		{
			get { return _machineOption; }
			set { _machineOption = value; }
		}

		public string MachineName
		{
			get { return _machineName; }
			set { _machineName = value; }
		}
	}
}
