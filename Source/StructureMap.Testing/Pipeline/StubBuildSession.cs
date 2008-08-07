using System;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    public class StubBuildSession : IBuildSession
    {
        #region IBuildSession Members

        public object CreateInstance(Type type, string name)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public Array CreateInstanceArray(Type pluginType, Instance[] instances)
        {
            throw new NotImplementedException();
        }

        public object CreateInstance(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public object ApplyInterception(Type pluginType, object actualValue)
        {
            return actualValue;
        }

        public InstanceBuilder FindBuilderByType(Type pluginType, Type pluggedType)
        {
            if (pluggedType == null)
            {
                return null;
            }

            InstanceBuilderList list = new InstanceBuilderList(pluginType, new Plugin[] {new Plugin(pluggedType),});
            return list.FindByType(pluggedType);
        }

        public InstanceBuilder FindBuilderByConcreteKey(Type pluginType, string concreteKey)
        {
            throw new NotImplementedException();
        }

        public void RegisterDefault(Type pluginType, object defaultObject)
        {
            
        }

        #endregion

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

        public InstanceBuilder FindInstanceBuilder(Type pluginType, Type pluggedType)
        {
            throw new NotImplementedException();
        }
    }
}