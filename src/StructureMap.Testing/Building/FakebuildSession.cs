using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap.Testing.Building
{
    public class FakeBuildSession : IBuildSession
    {
        public readonly Cache<Type, Cache<Instance, object>> LifecycledObjects =
            new Cache<Type, Cache<Instance, object>>(type => new Cache<Instance, object>());

        public readonly Cache<Type, object> Defaults = new Cache<Type, object>();
        public readonly Cache<Type, Cache<string, object>> NamedObjects = new Cache<Type, Cache<string, object>>(type => new Cache<string, object>());

        public FakeBuildSession()
        {
            Policies = new Policies();
        }

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

        public Policies Policies { get; private set; }

        public string RequestedName { get; set; }

        public void BuildUp(object target)
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>()
        {
            return (T) Defaults[typeof (T)];
        }

        public T GetInstance<T>(string name)
        {
            return (T) NamedObjects[typeof (T)][name];
        }

        public object GetInstance(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public object GetInstance(Type pluginType, string instanceKey)
        {
            throw new NotImplementedException();
        }

        public T TryGetInstance<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            throw new NotImplementedException();
        }

        public object TryGetInstance(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public object TryGetInstance(Type pluginType, string instanceKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> All<T>() where T : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAllInstances<T>()
        {
            return LifecycledObjects[typeof (T)].GetAll().Select(x => (T) x);
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            throw new NotImplementedException();
        }
    }
}