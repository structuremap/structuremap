using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    public interface ICommandCollection : IEnumerable
    {
        [IndexerName("Command")]
        ICommand this[string commandName] { get; }
    }
}