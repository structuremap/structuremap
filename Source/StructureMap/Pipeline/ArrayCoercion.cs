using System.Collections.Generic;
using System.Linq;

namespace StructureMap.Pipeline
{
    public class ArrayCoercion<T> : IEnumerableCoercion where T : class
    {
        public object Convert(IEnumerable<object> enumerable)
        {
            return enumerable.Select(x => x as T).ToArray();
        }
    }
}