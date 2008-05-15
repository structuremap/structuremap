using System;

namespace StructureMap.Pipeline
{
    public interface IBuildSession
    {
        object CreateInstance(Type pluginType, string name);
        object CreateInstance(Type pluginType, Instance instance);
        Array CreateInstanceArray(Type pluginType, Instance[] instances);
        object CreateInstance(Type pluginType);
        object ApplyInterception(Type pluginType, object actualValue);
        InstanceBuilder FindBuilderByType(Type pluginType, Type pluggedType);
        InstanceBuilder FindBuilderByConcreteKey(Type pluginType, string concreteKey);
    }
}