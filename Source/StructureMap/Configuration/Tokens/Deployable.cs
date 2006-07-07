using System;
using System.Collections;

namespace StructureMap.Configuration.Tokens
{
	[Serializable]
	public abstract class Deployable : GraphObject
	{
		public const string ALL = "All";

		private ArrayList _deploymentTargets;

		public Deployable() : base()
		{
			_deploymentTargets = new ArrayList();
		}

		public Deployable(string[] deploymentTargets) : this()
		{
			this.DeploymentTargets = deploymentTargets;
		}

		/// <summary>
		/// A string array of the valid deployment options for the GraphObject.
		/// </summary>
		public string[] DeploymentTargets
		{
			get { return (string[]) _deploymentTargets.ToArray(typeof (string)); }
			set
			{
				_deploymentTargets.Clear();
				_deploymentTargets.AddRange(value);
			}
		}

		/// <summary>
		/// Returns a boolean flag denoting whether or not the PluginGraphObject is deployed
		/// for the deploymentTarget
		/// </summary>
		/// <param name="deploymentTarget"></param>
		/// <returns></returns>
		public bool IsDeployed(string deploymentTarget)
		{
			if (_deploymentTargets.Count == 0 || _deploymentTargets.Contains(ALL))
			{
				return true;
			}

			return _deploymentTargets.Contains(deploymentTarget);
		}

		/// <summary>
		/// Simple string description of the deployment options for the PluginGraphObject
		/// </summary>
		public string DeploymentDescription
		{
			get
			{
				string[] targets = this.DeploymentTargets;
				if (targets.Length == 0)
				{
					return "All";
				}
				else
				{
					return string.Join(", ", targets);
				}
			}
		}
	}
}
