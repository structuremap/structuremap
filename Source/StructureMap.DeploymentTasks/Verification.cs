using System;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;
using StructureMap.Configuration;

namespace StructureMap.DeploymentTasks
{
	[TaskName("structuremap.verification")]
	public class Verification : Task
	{
		private string _configPath;
		private string _binPath = string.Empty;

		public Verification() : base()
		{
			
		}

		[TaskAttribute("configPath", Required=true)]
		public string ConfigPath
		{
			get { return _configPath; }
			set { _configPath = value; }
		}

		[TaskAttribute("binPath", Required=false)]
		public string BinPath
		{
			get { return _binPath; }
			set { _binPath = value; }
		}


		protected override void ExecuteTask()
		{
			if (_binPath == string.Empty)
			{
				_binPath = Path.GetDirectoryName(_configPath);
			}


			string msg = string.Format("StructureMap - Verifying configuration in {0}, searching for assemblies in {1}", _configPath, _binPath);
			Log(Level.Debug, msg);


			Problem[] problems = ProblemFinder.FindProblems(_configPath, _binPath);
			foreach (Problem problem in problems)
			{
				this.Log(Level.Error, problem.Path);
				this.Log(Level.Error, "\n");
				this.Log(Level.Error, problem.ToString());
				this.Log(Level.Error, "--------------------------------------------------");
			}

			if (problems.Length > 0)
			{
				throw new BuildException("StructureMap Errors Detected in file " + _configPath + ".");
			}
			else
			{
				this.Log(Level.Debug, "Success.");
			}

			
		}
	}
}