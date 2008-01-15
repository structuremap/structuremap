using System;
using System.IO;
using System.Reflection;

namespace StructureMap.DeploymentTasks.Versioning
{
    public class DotNetAssembly
    {
        private string _assemblyName;
        private byte[] _publicKey = new byte[0];
        private string _version;

        public DotNetAssembly()
        {
        }

        public DotNetAssembly(byte[] publicKey, string assemblyName, string version)
        {
            _publicKey = publicKey;
            _assemblyName = assemblyName;
            _version = version;
        }

        public DotNetAssembly(string assemblyName, string version)
        {
            _assemblyName = assemblyName;
            _version = version;
        }

        public DotNetAssembly(Assembly assembly)
        {
            _assemblyName = assembly.GetName().Name;
            _version = assembly.GetName().Version.ToString();
            _publicKey = assembly.GetName().GetPublicKey();
        }

        public byte[] PublicKey
        {
            get { return _publicKey; }
            set { _publicKey = value; }
        }

        public string AssemblyName
        {
            get { return _assemblyName; }
            set { _assemblyName = value; }
        }

        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

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