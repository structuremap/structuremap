using System;
using System.Collections.Generic;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
    public interface IDiagnosticInstance
    {
        bool CanBePartOfPluginFamily(PluginFamily family);
        Instance FindInstanceForProfile(PluginFamily family, string profileName, GraphLog log);
        InstanceToken CreateToken();
        void Preprocess(PluginFamily family);
    }

    public abstract class Instance : IDiagnosticInstance
    {
        private readonly string _originalName;
        private InstanceInterceptor _interceptor = new NulloInterceptor();
        private string _name = Guid.NewGuid().ToString();


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



        #region IDiagnosticInstance Members

        bool IDiagnosticInstance.CanBePartOfPluginFamily(PluginFamily family)
        {
            return canBePartOfPluginFamily(family);
        }

        Instance IDiagnosticInstance.FindInstanceForProfile(PluginFamily family, string profileName, GraphLog log)
        {
            return findMasterInstance(family, profileName, log);
        }

        InstanceToken IDiagnosticInstance.CreateToken()
        {
            return new InstanceToken(Name, getDescription());
        }

        void IDiagnosticInstance.Preprocess(PluginFamily family)
        {
            preprocess(family);
        }

        protected virtual void preprocess(PluginFamily family)
        {
            // no-op;
        }

        protected abstract string getDescription();

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
            object rawValue = createRawObject(pluginType, session);
            return applyInterception(rawValue, pluginType);
        }

        private object createRawObject(Type pluginType, IBuildSession session)
        {
            try
            {
                return build(pluginType, session);
            }
            catch (StructureMapException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new StructureMapException(400, ex);
            }
        }

        private object applyInterception(object rawValue, Type pluginType)
        {
            try
            {
                // Intercept with the Instance-specific InstanceInterceptor
                return _interceptor.Process(rawValue);
            }
            catch (Exception e)
            {
                throw new StructureMapException(270, e, Name, pluginType);
            }
        }

        protected abstract object build(Type pluginType, IBuildSession session);

        protected virtual Plugin findPlugin(PluginCollection plugins)
        {
            return null;
        }

        protected virtual Instance findMasterInstance(PluginFamily family, string profileName, GraphLog log)
        {
            return this;
        }

        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
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