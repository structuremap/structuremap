using System;
using System.Collections.Generic;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Testing.Building
{
    public class FakeBuildSession : IBuildSession
    {
        public readonly Cache<Type, Cache<Instance, object>> LifecycledObjects =
            new Cache<Type, Cache<Instance, object>>(type => new Cache<Instance, object>());

        public readonly Cache<Type, object> Defaults = new Cache<Type, object>();

        public void SetDefault<T>(T @object)
        {
            Defaults[typeof (T)] = @object;
        }

        public object BuildNewInSession(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public object BuildNewInOriginalContext(Type pluginType, Instance instance)
        {
            throw new NotImplementedException();
        }

        public object ResolveFromLifecycle(Type pluginType, Instance instance)
        {
            return LifecycledObjects[pluginType][instance];
        }

        public string RequestedName { get; set; }

        public T GetInstance<T>()
        {
            return (T) Defaults[typeof (T)];
        }
    }
}