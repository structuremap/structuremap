using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    [PluginFamily]
    public interface ICommand
    {
        string Name { get; set; }
        int Execute();

        [IndexerName("Parameter")]
        object this[string parameterName] { get; set; }

        void Attach(IDataSession session);
    }
}