using StructureMap.Client.TreeNodes;

namespace StructureMap.Client.Controllers
{
    public interface IApplicationShell
    {
        GraphObjectNode TopNode { get; set; }
        string ConfigurationPath { get; }
        string AssemblyFolder { get; }
        bool LockFolders { get; }
        void DisplayHTML(string html);
    }
}