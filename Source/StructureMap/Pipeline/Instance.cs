using System;
using System.Data;
using System.Web.UI;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public interface IInstanceCreator
    {
        object CreateInstance(Type type, string referenceKey);
        Array CreateInstanceArray(string pluginTypeName, Instance[] instances);
        object CreateInstance(string typeName);
        object CreateInstance(Type pluginType);

        object CreateInstance(Type pluginType, IConfiguredInstance instance);
        object ApplyInterception(Type pluginType, object actualValue);
    }

    public interface IDiagnosticInstance
    {
        bool CanBePartOfPluginFamily(PluginFamily family);
    }

    public abstract class Instance : IDiagnosticInstance
    {
        private string _name = Guid.NewGuid().ToString();
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

        public virtual object Build(Type pluginType, IInstanceCreator creator)
        {
            object rawValue = build(pluginType, creator);
            
            try
            {
                // Intercept with the Instance-specific InstanceInterceptor
                object interceptedValue = _interceptor.Process(rawValue);

                return creator.ApplyInterception(pluginType, interceptedValue);
            }
            catch (Exception e)
            {
                throw new StructureMapException(308, e, Name,
                                TypePath.GetAssemblyQualifiedName(rawValue.GetType()));
            }
        }

        protected abstract object build(Type pluginType, IInstanceCreator creator);


        bool IDiagnosticInstance.CanBePartOfPluginFamily(PluginFamily family)
        {
            return canBePartOfPluginFamily(family);
        }

        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
        }
    }

    public abstract class ExpressedInstance<T> : Instance
    {
        protected abstract T thisInstance { get;}

        public T WithName(string instanceKey)
        {
            Name = instanceKey;
            return thisInstance;
        }

        public T OnCreation<TYPE>(StartupHandler<TYPE> handler)
        {
            StartupInterceptor<TYPE> interceptor = new StartupInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }

        public T EnrichWith<TYPE>(EnrichmentHandler<TYPE> handler)
        {
            EnrichmentInterceptor<TYPE> interceptor = new EnrichmentInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }
    }
}