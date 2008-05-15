using System;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public interface IDiagnosticInstance
    {
        bool CanBePartOfPluginFamily(PluginFamily family);
        Instance FindMasterInstance(PluginFamily family);
    }

    public abstract class Instance : IDiagnosticInstance
    {
        private readonly string _originalName;
        private InstanceInterceptor _interceptor = new NulloInterceptor();
        private string _name = Guid.NewGuid().ToString();
        private Type _pluginType = typeof (object);


        protected Instance()
        {
            _originalName = _name;
        }

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

        internal Type PluginType
        {
            get { return _pluginType; }
            set { _pluginType = value; }
        }

        // TODO : remove pluginType from signature?

        #region IDiagnosticInstance Members

        bool IDiagnosticInstance.CanBePartOfPluginFamily(PluginFamily family)
        {
            return canBePartOfPluginFamily(family);
        }

        Instance IDiagnosticInstance.FindMasterInstance(PluginFamily family)
        {
            return findMasterInstance(family);
        }

        #endregion

        protected void replaceNameIfNotAlreadySet(string name)
        {
            if (_name == _originalName)
            {
                _name = name;
            }
        }

        public virtual object Build(Type pluginType, IBuildSession session)
        {
            object rawValue = build(pluginType, session);

            try
            {
                // Intercept with the Instance-specific InstanceInterceptor
                return _interceptor.Process(rawValue);
            }
            catch (Exception e)
            {
                throw new StructureMapException(308, e, Name,
                                                TypePath.GetAssemblyQualifiedName(rawValue.GetType()));
            }
        }

        protected abstract object build(Type pluginType, IBuildSession session);

        protected virtual Plugin findPlugin(PluginCollection plugins)
        {
            return null;
        }

        protected virtual Instance findMasterInstance(PluginFamily family)
        {
            return this;
        }

        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
        }

        public InstanceToken CreateToken()
        {
            throw new NotImplementedException();
        }

        internal virtual bool Matches(Plugin plugin)
        {
            return false;
        }
    }

    public abstract class ExpressedInstance<T> : Instance
    {
        protected abstract T thisInstance { get; }

        public T WithName(string instanceKey)
        {
            Name = instanceKey;
            return thisInstance;
        }

        public T OnCreation<TYPE>(Action<TYPE> handler)
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