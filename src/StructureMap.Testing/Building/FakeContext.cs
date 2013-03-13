using System;
using System.Collections.Generic;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Building
{
    public class FakeContext : IContext
    {
        public BuildStack BuildStack { get; private set; }
        public Type ParentType { get; private set; }
        public BuildFrame Root { get; private set; }
        public string RequestedName { get; private set; }
        public void BuildUp(object target)
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>()
        {
            throw new NotImplementedException();
        }

        public T GetInstance<T>(string name)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            throw new NotImplementedException();
        }
    }
}