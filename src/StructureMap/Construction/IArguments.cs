using System;

namespace StructureMap.Construction
{
    [Obsolete("Going to be unnecessary soon")]
    public interface IArguments
    {
        T Get<T>(string propertyName);
        bool Has(string propertyName);
    }
}