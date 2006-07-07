using System;

namespace StructureMap.Configuration.Tokens
{
	[Serializable]
	public class AssemblyToken : Deployable
	{
		private string _assemblyName = string.Empty;

		public AssemblyToken() : base()
		{
		}

		public AssemblyToken(string assemblyName, string[] deploymentTargets) : base(deploymentTargets)
		{
			_assemblyName = assemblyName;
		}

		public string AssemblyName
		{
			get { return _assemblyName; }
			set { _assemblyName = value; }
		}

		public override string ToString()
		{
			return string.Format("Assembly:  {0}, Deployed to {1}", _assemblyName, this.DeploymentTargets);
		}

		public override bool Equals(object obj)
		{
			AssemblyToken peer = obj as AssemblyToken;
			if (peer == null)
			{
				return false;
			}

			return this.AssemblyName == peer.AssemblyName;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public void MarkLoadFailure(Exception ex)
		{
			Problem problem = new Problem(ConfigurationConstants.COULD_NOT_LOAD_ASSEMBLY, ex);
			this.LogProblem(problem);
		}

		public override void AcceptVisitor(IConfigurationVisitor visitor)
		{
			visitor.HandleAssembly(this);
		}

		protected override string key
		{
			get { return this.AssemblyName; }
		}

	}
}
