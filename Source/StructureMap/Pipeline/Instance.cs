using System;
using System.Data;
using System.Web.UI;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public interface IInstanceCreator
    {
        object CreateInstance(Type type, string referenceKey);
        Array CreateInstanceArray(string pluginType, InstanceMemento[] instanceMementoes);
        object CreateInstance(string typeName, InstanceMemento memento);
        object CreateInstance(string typeName);
        object CreateInstance(Type type);
        InstanceInterceptor FindInterceptor(Type type);
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

        public virtual object Build(Type type, IInstanceCreator creator)
        {
            object rawValue = build(type, creator);
            
            // Intercept with the Instance-specific InstanceInterceptor
            object interceptedValue = _interceptor.Process(rawValue);

            // Now, give the at large Interceptors a chance to intercept
            return creator.FindInterceptor(interceptedValue.GetType()).Process(interceptedValue);
        }

        protected abstract object build(Type type, IInstanceCreator creator);

        //public abstract void Diagnose<T>(IInstanceCreator creator, IInstanceDiagnostics diagnostics) where T : class;
        //public abstract void Describe<T>(IInstanceDiagnostics diagnostics) where T : class;
    }
}