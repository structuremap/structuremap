using System;
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
    }

    public abstract class Instance : HasScope, IDiagnosticInstance
    {
        private readonly string _originalName;
        private InstanceInterceptor _interceptor = new NulloInterceptor();
        private string _name = Guid.NewGuid().ToString();


        private PluginFamily _parent;

        protected Instance()
        {
            _originalName = _name;
        }

        public PluginFamily Parent
        {
            get { return _parent; }
            internal set
            {
                _parent = value;
                scopedParent = _parent;
            }
        }

        protected virtual bool doesRecordOnTheStack
        {
            get { return true; }
        }

        public InstanceInterceptor Interceptor
        {
            get { return _interceptor; }
            set { _interceptor = value; }
        }

        internal bool IsReference { get; set; }
        internal bool CopyAsIsWhenClosingInstance { get; set; }

        #region IDiagnosticInstance Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        internal Type ConcreteType
        {
            get { return getConcreteType(null); }
        }

        internal string Description
        {
            get { return getDescription(); }
        }

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

        #endregion

        public virtual object Build(Type pluginType, BuildSession session)
        {
            // "Build" the desired object
            object rawValue = createRawObject(pluginType, session);

            // Allow the Interceptor a chance to enhance, configure,  
            // wrap with a decorator, or even replace the rawValue
            object finalValue = applyInterception(rawValue, pluginType, session);

            return finalValue;
        }

        private object createRawObject(Type pluginType, BuildSession session)
        {
            try
            {
                return build(pluginType, session);
            }
            catch (StructureMapException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new StructureMapException(400, ex);
            }
        }

        protected virtual Type getConcreteType(Type pluginType)
        {
            return pluginType;
        }

        protected abstract string getDescription();

        protected void replaceNameIfNotAlreadySet(string name)
        {
            if (_name == _originalName)
            {
                _name = name;
            }
        }


        private object applyInterception(object rawValue, Type pluginType, IContext context)
        {
            try
            {
                // Intercept with the Instance-specific InstanceInterceptor
                return _interceptor.Process(rawValue, context);
            }
            catch (Exception e)
            {
                throw new StructureMapException(270, e, Name, pluginType);
            }
        }

        [CLSCompliant(false)]
        protected virtual object build(Type pluginType, BuildSession session)
        {
            throw new NotImplementedException();
        }

        protected virtual Instance findMasterInstance(PluginFamily family, string profileName, GraphLog log)
        {
            return this;
        }

        [CLSCompliant(false)]
        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
        }


        internal virtual bool Matches(Plugin plugin)
        {
            return false;
        }

        public virtual Instance CloseType(Type[] types)
        {
            return null;
        }

        public int InstanceKey(Type pluginType)
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0)*397) ^
                       (pluginType != null ? pluginType.AssemblyQualifiedName.GetHashCode() : 0);
            }
        }

        public bool IsUnique()
        {
            return Lifecycle is UniquePerRequestLifecycle;
        }

        public PluginGraph Owner()
        {
            if (Parent == null || Parent.Owner == null) return null;

            var owner = Parent.Owner;
            while (owner.Parent != null)
            {
                owner = owner.Parent;
            }

            return owner;
        }
    }

    /// <summary>
    ///     Base class for many of the Instance subclasses to support
    ///     method chaining in the Registry DSL for common options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ExpressedInstance<T> : Instance
    {
        protected abstract T thisInstance { get; }


        /// <summary>
        ///     Set the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public T Named(string instanceKey)
        {
            Name = instanceKey;
            return thisInstance;
        }

        /// <summary>
        ///     Register an Action to perform on the object created by this Instance
        ///     before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T OnCreation<THandler>(Action<THandler> handler)
        {
            var interceptor = new StartupInterceptor<THandler>((c, o) => handler(o));
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(EnrichmentHandler<THandler> handler)
        {
            var interceptor = new EnrichmentInterceptor<THandler>((c, o) => handler(o));
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register a Func to potentially enrich or substitute for the object
        ///     created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="THandler"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<THandler>(ContextEnrichmentHandler<THandler> handler)
        {
            var interceptor = new EnrichmentInterceptor<THandler>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        ///     Register an <see cref="InstanceInterceptor">InstanceInterceptor</see> with this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public T InterceptWith(InstanceInterceptor interceptor)
        {
            Interceptor = interceptor;
            return thisInstance;
        }
    }
}