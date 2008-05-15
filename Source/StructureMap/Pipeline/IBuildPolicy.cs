using System;

namespace StructureMap.Pipeline
{
    public interface IBuildPolicy
    {
        object Build(IBuildSession buildSession, Type pluginType, Instance instance);
        IBuildPolicy Clone();
    }
}