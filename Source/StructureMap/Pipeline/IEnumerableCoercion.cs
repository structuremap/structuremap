using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface IEnumerableCoercion
    {
        object Convert(IEnumerable<object> enumerable);
    }
}