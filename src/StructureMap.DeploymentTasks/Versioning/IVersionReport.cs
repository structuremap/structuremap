namespace StructureMap.DeploymentTasks.Versioning
{
    public interface IVersionReport
    {
        void MissingAssembly(string assemblyName, string version);
        void VersionMismatchAssembly(string assemblyName, string expectedVersion, string actualVersion);
        void MissingFile(string fileName);
        void VersionMismatchFile(string fileName);
    }
}