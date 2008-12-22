using System;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{

    public interface IInstance
    {
        string Name { get; }
        
        /// <summary>
        /// The actual concrete type of this Instance.  Not every type of IInstance
        /// can determine the ConcreteType
        /// </summary>
        Type ConcreteType { get; }


        string Description { get; }
    }

    public interface IDiagnosticInstance : IInstance
    {
        bool CanBePartOfPluginFamily(PluginFamily family);
        Instance FindInstanceForProfile(PluginFamily family, string profileName, GraphLog log);
        InstanceToken CreateToken();
        void Preprocess(PluginFamily family);
        void AddTemplatedInstanceTo(PluginFamily family, Type[] templateTypes);
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

        protected virtual bool doesRecordOnTheStack
        {
            get { return true; }
        }

        public InstanceInterceptor Interceptor
        {
            get { return _interceptor; }
            set { _interceptor = value; }
        }

        #region IDiagnosticInstance Members

        public string Name
        {
            get { return _name; }
            set { _name = value; }
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

        void IDiagnosticInstance.Preprocess(PluginFamily family)
        {
            preprocess(family);
        }

        void IDiagnosticInstance.AddTemplatedInstanceTo(PluginFamily family, Type[] templateTypes)
        {
            addTemplatedInstanceTo(family, templateTypes);
        }

        Type IInstance.ConcreteType
        {
            get { return getConcreteType(null); }
        }

        string IInstance.Description
        {
            get { return getDescription(); }
        }

        #endregion

        public virtual object Build(Type pluginType, BuildSession session)
        {
            markBuildStackStart(session, pluginType);

            // "Build" the desired object
            object rawValue = createRawObject(pluginType, session);
            
            // Allow the Interceptor a chance to enhance, configure,  
            // wrap with a decorator, or even replace the rawValue
            object finalValue = applyInterception(rawValue, pluginType);

            markBuildStackFinish(session);

            return finalValue;
        }

        protected virtual void markBuildStackFinish(BuildSession session)
        {
            if (!doesRecordOnTheStack) return;

            session.BuildStack.Pop();
        }

        protected virtual void markBuildStackStart(BuildSession session, Type pluginType)
        {
            if (!doesRecordOnTheStack) return;

            session.BuildStack.Push(new BuildFrame(pluginType, Name, getConcreteType(pluginType)));
        }

        private object createRawObject(Type pluginType, BuildSession session)
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

        protected virtual Type getConcreteType(Type pluginType)
        {
            return pluginType;
        }

        protected virtual void addTemplatedInstanceTo(PluginFamily family, Type[] templateTypes)
        {
            if (canBePartOfPluginFamily(family))
            {
                family.AddInstance(this);
            }
        }

        protected virtual void preprocess(PluginFamily family)
        {
            // no-op;
        }

        protected abstract string getDescription();

        protected void replaceNameIfNotAlreadySet(string name)
        {
            if (_name == _originalName)
            {
                _name = name;
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

        protected abstract object build(Type pluginType, BuildSession session);

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

    /// <summary>
    /// Base class for many of the Instance subclasses to support 
    /// method chaining in the Registry DSL for common options
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ExpressedInstance<T> : Instance
    {
        protected abstract T thisInstance { get; }


        /// <summary>
        /// Set the name of this Instance
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public T WithName(string instanceKey)
        {
            Name = instanceKey;
            return thisInstance;
        }

        /// <summary>
        /// Register an Action to perform on the object created by this Instance
        /// before it is returned to the caller
        /// </summary>
        /// <typeparam name="TYPE"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T OnCreation<TYPE>(Action<TYPE> handler)
        {
            var interceptor = new StartupInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        /// Register a Func to potentially enrich or substitute for the object
        /// created by this Instance before it is returned to the caller
        /// </summary>
        /// <typeparam name="TYPE"></typeparam>
        /// <param name="handler"></param>
        /// <returns></returns>
        public T EnrichWith<TYPE>(EnrichmentHandler<TYPE> handler)
        {
            var interceptor = new EnrichmentInterceptor<TYPE>(handler);
            Interceptor = interceptor;

            return thisInstance;
        }

        /// <summary>
        /// Register an <see cref="InstanceInterceptor">InstanceInterceptor</see> with this Instance
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