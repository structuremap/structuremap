using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    [PluginFamily]
    public interface ICommand
    {
        string Name { get; set; }

        [IndexerName("Parameter")]
        object this[string parameterName] { get; set; }

        int Execute();

        void Attach(IDataSession session);
    }
}