using System;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Configuration.DSL.Expressions
{
    public abstract class MementoBuilder<T> : IExpression, IMementoBuilder
    {
        protected readonly Type _pluginType;
        protected List<IExpression> _children = new List<IExpression>();
        private string _instanceKey = null;

        public MementoBuilder(Type pluginType)
        {
            _pluginType = pluginType;
            buildMemento();
            memento.InstanceKey = Guid.NewGuid().ToString();
        }

        protected abstract InstanceMemento memento { get; }

        protected abstract T thisInstance { get; }

        public string InstanceKey
        {
            get { return memento.InstanceKey; }
            set { memento.InstanceKey = value; }
        }

        internal Type PluginType
        {
            get { return _pluginType; }
        }

        #region IExpression Members

        void IExpression.Configure(PluginGraph graph)
        {
            validate();
            PluginFamily family = graph.LocateOrCreateFamilyForType((Type) _pluginType);
            configureMemento(family);

            if (!string.IsNullOrEmpty(_instanceKey))
            {
                memento.InstanceKey = _instanceKey;
            }

            family.Source.AddExternalMemento(memento);

            foreach (IExpression child in _children)
            {
                child.Configure(graph);
            }
        }

        #endregion

        #region IMementoBuilder Members

        InstanceMemento IMementoBuilder.BuildMemento(PluginFamily family)
        {
            return buildMementoFromFamily(family);
        }

        InstanceMemento IMementoBuilder.BuildMemento(PluginGraph graph)
        {
            PluginFamily family = graph.LocateOrCreateFamilyForType(_pluginType);
            return buildMementoFromFamily(family);
        }

        public void SetInstanceName(string instanceKey)
        {
            _instanceKey = instanceKey;
        }

        public abstract void ValidatePluggability(Type pluginType);

        #endregion

        protected abstract void configureMemento(PluginFamily family);

        protected abstract void validate();

        public T WithName(string instanceKey)
        {
            memento.InstanceKey = instanceKey;
            return thisInstance;
        }

        public T OnCreation<TYPE>(StartupHandler<TYPE> handler)
        {
            StartupInterceptor<TYPE> interceptor = new StartupInterceptor<TYPE>(handler);
            memento.Interceptor = interceptor;

            return thisInstance;
        }

        public T EnrichWith<TYPE>(EnrichmentHandler<TYPE> handler)
        {
            EnrichmentInterceptor<TYPE> interceptor = new EnrichmentInterceptor<TYPE>(handler);
            memento.Interceptor = interceptor;

            return thisInstance;
        }

        protected abstract void buildMemento();

        private InstanceMemento buildMementoFromFamily(PluginFamily family)
        {
            validate();
            configureMemento(family);
            return memento;
        }


        protected void addChildExpression(IExpression expression)
        {
            _children.Add(expression);
        }
    }
}