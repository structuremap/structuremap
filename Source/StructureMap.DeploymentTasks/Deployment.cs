using System;
using System.IO;
using System.Security.Permissions;
using NAnt.Core;
using NAnt.Core.Attributes;
using StructureMap.Graph;

namespace StructureMap.DeploymentTasks
{
	[EnvironmentPermission(SecurityAction.Assert, Read="COMPUTERNAME"), TaskName("structuremap.deployment")]
	[FileIOPermission(SecurityAction.Assert)]
	public class Deployment : Task
	{
		private DeploymentConfiguration _configuration = new DeploymentConfiguration();

		public Deployment()
		{
			_configuration.MachineName = Environment.MachineName;
		}


		[TaskAttribute("configPath", Required=true)]
		public string ConfigPath
		{
			get { return _configuration.ConfigPath; }
			set { _configuration.ConfigPath = Path.GetFullPath(value); }
		}

		[TaskAttribute("destinationPath", Required=true)]
		public string DestinationPath
		{
			get { return _configuration.DestinationPath; }
			set { _configuration.DestinationPath = Path.GetFullPath(value); }
		}

		[TaskAttribute("deploymentTarget", Required=false)]
		public string DeploymentTarget
		{
			get { return _configuration.DeploymentTarget; }
			set { _configuration.DeploymentTarget = value; }
		}

		[TaskAttribute("profile", Required=false)]
		public string Profile
		{
			get { return _configuration.Profile; }
			set { _configuration.Profile = value; }
		}

		[TaskAttribute("machineOption", Required=false)]
		public string MachineOption
		{
			get { return _configuration.MachineOption.ToString("g"); }
			set { _configuration.MachineOption = (MachineSpecificOption) Enum.Parse(typeof (MachineSpecificOption), value); }
		}

		public MachineSpecificOption MachineSpecificOption
		{
			get
			{
				return _configuration.MachineOption;
			}
			set
			{
				_configuration.MachineOption = value;
			}
		}	


		protected override void ExecuteTask()
		{
			Console.WriteLine();
			string msg = string.Format("Deploying {0} to {1}",_configuration.ConfigPath, _configuration.DestinationPath);
			Console.WriteLine(msg);
			Console.Out.WriteLine("DeploymentTarget = " + _configuration.DeploymentTarget);
			Console.Out.WriteLine("Profile = " + _configuration.Profile);
			Console.Out.WriteLine("MachineOption = " + _configuration.MachineOption.ToString("g"));

			CreateConfiguration();

			Console.WriteLine("Success.");
			writeSeparator();
		}

		public void CreateConfiguration()
		{
			RemoteGraphContainer container = new RemoteGraphContainer(_configuration.ConfigPath);
			RemoteGraph remoteGraph = container.GetRemoteGraph();
			DeploymentExecutor executor = remoteGraph.CreateDeploymentExecutor();
			executor.Execute(_configuration);
		}

		private void writeSeparator()
		{
			Console.WriteLine("-----------------------------------------------------------------------------------------------");
		}

		public string MachineName
		{
			get
			{
				return _configuration.MachineName;
			}
			set { _configuration.MachineName = value; }
		}


	}


}