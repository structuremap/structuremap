using System;

namespace StructureMap.Pipeline
{
    public interface IObjectBuilder
    {
        object Resolve(Type pluginType, Instance instance, BuildSession session, IPipelineGraph pipelineGraph);
        object ConstructNew(Type pluginType, Instance instance, BuildSession session);

        object ApplyInterception(Type pluginType, object actualValue, BuildSession session,
                                 Instance instance);

    }
}