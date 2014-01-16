using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Building;
using StructureMap.Building.Interception;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.TypeRules;

namespace StructureMap.Pipeline
{
    public abstract class Instance : HasScope, IDiagnosticInstance, IDescribed
    {
        private readonly string _originalName;
        private string _name = Guid.NewGuid().ToString();


        private PluginFamily _parent;

        private readonly IList<IInterceptor> _interceptors = new List<IInterceptor>();

        public void AddInterceptor(IInterceptor interceptor)
        {
            // TODO -- defensive check to blow up if the interceptor "Accepts" cannot handle the returned type
            _interceptors.Add(interceptor);
        }

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

        public virtual IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            return ToDependencySource(pluginType);
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

        public Policies Policies
        {
            get
            {
                if (_parent == null) return new Policies();

                return _parent.Root.Policies;
            }
        }

        public IEnumerable<IInterceptor> Interceptors
        {
            get { return _interceptors; }
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

        public string Description
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

        [Obsolete("Just fold this into ConcreteType")]
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

        public bool HasExplicitName()
        {
            return _name != _originalName;
        }

        [CLSCompliant(false)]
        protected virtual bool canBePartOfPluginFamily(PluginFamily family)
        {
            return true;
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

        public IBuildPlan CreatePlan(Type pluginType, Policies policies)
        {
            // TODO -- memoize this please!
            var builderSource = ToBuilder(pluginType, policies);
            var interceptors = policies.Interceptors.SelectInterceptors(ConcreteType).Union(_interceptors);
            return new BuildPlan(pluginType, this, builderSource, interceptors);
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