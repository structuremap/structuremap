using System;
using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class ListCoercion<T> : IEnumerableCoercion where T : class
    {
        public object Convert(IEnumerable<object> enumerable)
        {
            return enumerable.Select(x => x as T).ToList();
        }

        public Type ElementType { get { return typeof (T); } }
    }
}