using System;
using StructureMap.Building;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Interceptors;

namespace StructureMap.Pipeline
{
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

        /// <summary>
        /// Strategy for how this Instance would be built as
        /// an inline dependency in the parent Instance's
        /// "Build Plan"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public abstract IDependencySource ToDependencySource(Type pluginType);

        public PluginFamily Parent
        {
            get { return _parent; }
            internal set
            {
                _parent = value;
                scopedParent = _parent;
            }
        }

        public Policies Policies
        {
            get
            {
                return _parent.Root.Policies;
            }
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

        [CLSCompliant(false)]
        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
        }


        internal virtual bool Matches(Plugin plugin)
        {
            return false;
        }

        /// <summary>
        /// Return the closed type value for this Instance
        /// when starting from an open generic type
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public virtual Instance CloseType(Type[] types)
        {
            return this;
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
}