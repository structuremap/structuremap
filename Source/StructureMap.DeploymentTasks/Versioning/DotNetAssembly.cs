using System;
using System.IO;
using System.Reflection;

namespace StructureMap.DeploymentTasks.Versioning
{
    public class DotNetAssembly
    {
        private byte[] _publicKey = new byte[0];

        public DotNetAssembly()
        {
        }

        public DotNetAssembly(byte[] publicKey, string assemblyName, string version)
        {
            _publicKey = publicKey;
            AssemblyName = assemblyName;
            Version = version;
        }

        public DotNetAssembly(string assemblyName, string version)
        {
            AssemblyName = assemblyName;
            Version = version;
        }

        public DotNetAssembly(Assembly assembly)
        {
            AssemblyName = assembly.GetName().Name;
            Version = assembly.GetName().Version.ToString();
            _publicKey = assembly.GetName().GetPublicKey();
        }

        public byte[] PublicKey
        {
            get { return _publicKey; }
            set { _publicKey = value; }
        }

        public string AssemblyName { get; set; }

        public string Version { get; set; }

        public static DotNetAssembly TryCreateAssembly(FileInfo fileInfo)
        {
            try
            {
                Assembly assembly = Assembly.LoadFile(fileInfo.FullName);
                return new DotNetAssembly(assembly);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void CheckVersion(DeployedDirectory directory, IVersionReport report)
        {
            DotNetAssembly deployedVersion = directory.FindAssembly(AssemblyName);

            if (deployedVersion == null)
            {
                report.MissingAssembly(AssemblyName, Version);
            }
            else
            {
                if (Version != deployedVersion.Version)
                {
                    report.VersionMismatchAssembly(AssemblyName, Version, deployedVersion.Version);
                }
            }
        }
    }
}