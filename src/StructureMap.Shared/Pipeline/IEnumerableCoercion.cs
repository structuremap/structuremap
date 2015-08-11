using System;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface IEnumerableCoercion
    {
        Type ElementType { get; }
        object Convert(IEnumerable<object> enumerable);
    }
}