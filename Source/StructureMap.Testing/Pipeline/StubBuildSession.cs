using System;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    public class StubBuildSession : BuildSession
    {
        #region BuildSession Members

        public StubBuildSession() : base(new PluginGraph())
        {
        }

        public override object ApplyInterception(Type pluginType, object actualValue)
        {
            return actualValue;
        }

        public InstanceBuilder FindBuilderByType(Type pluginType, Type pluggedType)
        {
            if (pluggedType == null)
            {
                return null;
            }

            InstanceBuilderList list = new InstanceBuilderList(pluginType, new[] {new Plugin(pluggedType),});
            return list.FindByType(pluggedType);
        }

        public InstanceBuilder FindBuilderByConcreteKey(Type pluginType, string concreteKey)
        {
            throw new NotImplementedException();
        }


        public new BuildStack BuildStack
        {
            get
            {
                var stack = new BuildStack();
                stack.Push(new BuildFrame(typeof(Rule), "Blue", typeof(ColorRule)));
                return stack;
            }
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