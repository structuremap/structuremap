using System.Collections.Generic;

namespace StructureMap.Graph
{
    /// <summary>
    /// Base class for PluginGraphObject classes that can be marked for deployment
    /// targets.
    /// </summary>
    public abstract class Deployable
    {
        public const string ALL = "All";

        private List<string> _deploymentTargets;

        public Deployable()
        {
            _deploymentTargets = new List<string>();
        }

        public Deployable(string[] deploymentTargets) : this()
        {
            DeploymentTargets = deploymentTargets;
        }

        /// <summary>
        /// A string array of the valid deployment options for the PluginGraphObject.
        /// </summary>
        public string[] DeploymentTargets
        {
            get { return _deploymentTargets.ToArray(); }
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
                string[] targets = DeploymentTargets;
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