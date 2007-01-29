using System.Collections;
using System.Runtime.CompilerServices;

namespace StructureMap.DataAccess
{
    public interface IReaderSourceCollection : IEnumerable
    {
        int Count { get; }

        [IndexerName("ReaderSource")]
        IReaderSource this[string name] { get; }
    }
}