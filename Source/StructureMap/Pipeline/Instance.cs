using System;
using System.Data;
using System.Web.UI;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public interface IInstanceCreator
    {
        T CreateInstance<T>(string referenceKey);
        T CreateInstance<T>();
        Array CreateInstanceArray(string pluginType, InstanceMemento[] instanceMementoes);
        object CreateInstance(string typeName, InstanceMemento memento);
        object CreateInstance(string typeName);
    }

    public interface IInstanceDiagnostics
    {
    }

    public abstract class Instance
    {
        private string _name;
        private InstanceInterceptor _interceptor = new NulloInterceptor();

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public InstanceInterceptor Interceptor
        {
            get { return _interceptor; }
            set { _interceptor = value; }
        }

        public T Build<T>(IInstanceCreator creator) where T : class
        {
            T rawValue = build<T>(creator);
            return (T) _interceptor.Process(rawValue);
        }

        protected abstract T build<T>(IInstanceCreator creator) where T : class;

        public abstract void Diagnose<T>(IInstanceCreator creator, IInstanceDiagnostics diagnostics) where T : class;
        public abstract void Describe<T>(IInstanceDiagnostics diagnostics) where T : class;
    }
}