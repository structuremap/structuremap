using System;

namespace StructureMap.Pipeline
{
    /// <summary>
    /// An object that specifies a "Policy" about how Instance's are invoked.
    /// </summary>
    public interface IBuildPolicy
    {
        object Build(BuildSession buildSession, Type pluginType, Instance instance);
        IBuildPolicy Clone();
        void EjectAll();
    }
}