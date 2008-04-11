using System;
using System.Collections.Generic;
using System.Text;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    public class StubInstanceCreator : StructureMap.Pipeline.IInstanceCreator
    {
        public object CreateInstance(Type type, string referenceKey)
        {
            throw new NotImplementedException();
        }

        public Array CreateInstanceArray(string pluginTypeName, Instance[] instances)
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

        public object CreateInstance(Type pluginType)
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

        public InstanceBuilder FindInstanceBuilder(Type pluginType, Type pluggedType)
        {
            throw new NotImplementedException();
        }


        public object ApplyInterception(Type pluginType, object actualValue)
        {
            return actualValue;
        }
    }
}
