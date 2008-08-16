using System;

namespace StructureMap.Pipeline
{
    public interface IBuildPolicy
    {
        object Build(BuildSession buildSession, Type pluginType, Instance instance);
        IBuildPolicy Clone();
    }
}