using System;
using System.Collections;
using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface IEnumerableCoercion
    {
        object Convert(IEnumerable<object> enumerable);
        Type ElementType { get; }
    }
}