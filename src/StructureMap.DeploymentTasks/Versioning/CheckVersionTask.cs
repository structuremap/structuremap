using System.Collections;
using System.IO;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks.Versioning
{
    [TaskName("structuremap.checkversion")]
    public class CheckVersionTask : Task, IVersionReport
    {
        private readonly ArrayList _exclusionList = new ArrayList();
        private string _manifestFile;
        private bool _succeeded = true;
        private string _targetFolder;

        [TaskAttribute("manifest", Required = true)]
        public string ManifestFile { get { return _manifestFile; } set { _manifestFile = value; } }

        [TaskAttribute("directory", Required = true)]
        public string TargetFolder { get { return _targetFolder; } set { _targetFolder = value; } }

        [TaskAttribute("exclusions", Required = false)]
        public string Exclusions
        {
            set
            {
                string[] exclusions = value.Split(',');
                foreach (string exclusion in exclusions)
                {
                    _exclusionList.Add(exclusion.Trim().ToUpper());
                }
            }
            get { return ""; }
        }

        #region IVersionReport Members

        public void MissingAssembly(string assemblyName, string version)
        {
            _succeeded = false;
            string message = string.Format("Expected Assembly {0}, Version {1} is missing", assemblyName, version);
            Log(Level.Error, message);
        }

        public void VersionMismatchAssembly(string assemblyName, string expectedVersion, string actualVersion)
        {
            _succeeded = false;
            string message =
                string.Format("Assembly Version Mismatch for Assembly {0}, Expected version {1}, found version {2}",
                              assemblyName, expectedVersion, actualVersion);
            Log(Level.Error, message);
        }

        public void MissingFile(string fileName)
        {
            if (isExcluded(fileName))
            {
                return;
            }

            _succeeded = false;
            string message = string.Format("Missing file {0}", fileName);
            Log(Level.Error, message);
        }

        public void VersionMismatchFile(string fileName)
        {
            if (isExcluded(fileName))
            {
                return;
            }

            _succeeded = false;
            string message = string.Format("Version mismatch detected for file {0}.  Check contents", fileName);
            Log(Level.Error, message);
        }

        #endregion

        protected override void ExecuteTask()
        {
            _succeeded = true;

            Log(Level.Info,
                string.Format("Starting version checking of folder {0} against manifest file {1}", _targetFolder,
                              _manifestFile));

            var targetDirectoryInfo = new DirectoryInfo(TargetFolder);
            var actualDirectory = new DeployedDirectory(targetDirectoryInfo);

            DeployedDirectory expectedDirectory = DeployedDirectory.ReadFromXml(_manifestFile);

            expectedDirectory.CheckDeployedVersions(actualDirectory, this);

            if (!_succeeded)
            {
                string message = string.Format("Version checking for {0} Failed!", _targetFolder);
                throw new BuildException(message);
            }
        }

        private bool isExcluded(string fileName)
        {
            return (_exclusionList.Contains(fileName.Trim().ToUpper()));
        }
    }
}