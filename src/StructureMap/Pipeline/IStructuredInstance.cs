using System;

namespace StructureMap.Pipeline
{
    [Obsolete("Try to get rid of this")]
    public interface IStructuredInstance
    {
        Instance GetChild(string name);
        Instance[] GetChildArray(string name);
        void RemoveKey(string name);
    }
}