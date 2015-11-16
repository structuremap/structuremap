using System.Collections.Generic;

namespace StructureMap.Pipeline
{
    public interface ITransientTracking : IObjectCache
    {
        void Release(object o);

        IEnumerable<object> Tracked { get; }
    }
}