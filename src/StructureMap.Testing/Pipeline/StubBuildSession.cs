using System;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    public class StubBuildSession : BuildSession
    {
        private readonly static PluginGraph _pluginGraph = new PluginGraph();
        private static readonly PipelineGraph _pipeline;

        static StubBuildSession()
        {
            _pipeline = new PipelineGraph(_pluginGraph);
            
        }

        public StubBuildSession()
            : base(_pipeline, new ObjectBuilder(_pipeline, _pluginGraph.InterceptorLibrary))
        {
        }

        public new BuildStack BuildStack
        {
            get
            {
                var stack = new BuildStack();
                stack.Push(new BuildFrame(typeof (Rule), "Blue", typeof (ColorRule)));
                return stack;
            }
        }


        public InstanceBuilder FindBuilderByConcreteKey(Type pluginType, string concreteKey)
        {
            throw new NotImplementedException();
        }


        public object CreateInstance(string typeName, IConfiguredInstance instance)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(string typeName)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type pluginType, IConfiguredInstance instance)
        {
            throw new NotImplementedException();
        }

        public InstanceBuilder FindInstanceBuilder(Type pluginType, string concreteKey)
        {
            throw new NotImplementedException();
        }

        public InstanceBuilder FindInstanceBuilder(Type pluginType, Type TPluggedType)
        {
            throw new NotImplementedException();
        }
    }
}