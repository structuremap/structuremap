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

        protected override string key
        {
            get { return AssemblyName; }
        }

        public override string ToString()
        {
            return string.Format("Assembly:  {0}, Deployed to {1}", _assemblyName, DeploymentTargets);
        }

        public override bool Equals(object obj)
        {
            AssemblyToken peer = obj as AssemblyToken;
            if (peer == null)
            {
                return false;
            }

            return AssemblyName == peer.AssemblyName;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void MarkLoadFailure(Exception ex)
        {
            Problem problem = new Problem(ConfigurationConstants.COULD_NOT_LOAD_ASSEMBLY, ex);
            LogProblem(problem);
        }

        public override void AcceptVisitor(IConfigurationVisitor visitor)
        {
            visitor.HandleAssembly(this);
        }
    }
}