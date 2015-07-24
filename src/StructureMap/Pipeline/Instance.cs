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
    public abstract class Instance : HasLifecycle, IDescribed
    {
		public class MissingInstance : Instance
		{
			public Instance InnerInstance { get; private set; }
			public string RequestedName { get; private set; }

			public MissingInstance(string requestedName, Instance innerInstance)
			{
				InnerInstance = innerInstance;
				RequestedName = requestedName;
				_originalName += "++" + requestedName + "++";
			}

			public override IDependencySource ToDependencySource(Type pluginType)
			{
				return InnerInstance.ToDependencySource(pluginType);
			}

			public override string Description
			{
				get { return InnerInstance.Description; }
			}

			public override Type ReturnedType
			{
				get { return InnerInstance.ReturnedType; }
			}

			protected override IBuildPlan buildPlan(Type pluginType, Policies policies)
			{
				return InnerInstance.buildPlan(pluginType, policies);
			}

			public override IDependencySource ToBuilder(Type pluginType, Policies policies)
			{
				return InnerInstance.ToBuilder(pluginType, policies);
			}

			public override Instance CloseType(Type[] types)
			{
				return InnerInstance.CloseType(types);
			}
		}

        private string _originalName;
        private string _name;

        private readonly IList<IInterceptor> _interceptors = new List<IInterceptor>();

        /// <summary>
        /// Add an interceptor to only this Instance
        /// </summary>
        /// <param name="interceptor"></param>
        public void AddInterceptor(IInterceptor interceptor)
        {
            if (ReturnedType != null && !ReturnedType.CanBeCastTo(interceptor.Accepts))
            {
                throw new ArgumentOutOfRangeException(
                    "ReturnedType {0} cannot be cast to the Interceptor Accepts type {1}".ToFormat(
                        ReturnedType.GetFullName(), interceptor.Accepts.GetFullName()));
            }

            _interceptors.Add(interceptor);
        }

        protected Instance()
        {
            Id = Guid.NewGuid();
            _originalName = _name = Id.ToString();
        }

        /// <summary>
        /// Strategy for how this Instance would be built as
        /// an inline dependency in the parent Instance's
        /// "Build Plan"
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public abstract IDependencySource ToDependencySource(Type pluginType);

        /// <summary>
        /// Creates an IDependencySource that can be used to build the object
        /// represented by this Instance 
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="policies"></param>
        /// <returns></returns>
        public virtual IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            return ToDependencySource(pluginType);
        }

        public IEnumerable<IInterceptor> Interceptors
        {
            get { return _interceptors; }
        }

        public virtual string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public abstract string Description { get; }

        public InstanceToken CreateToken()
        {
            return new InstanceToken(Name, Description);
        }

        /// <summary>
        /// The known .Net Type built by this Instance.  May be null when indeterminate.
        /// </summary>
        public abstract Type ReturnedType { get; }

        /// <summary>
        /// Does this Instance have a user-defined name?
        /// </summary>
        /// <returns></returns>
        public bool HasExplicitName()
        {
            return _name != _originalName;
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

        private readonly object _buildLock = new object();
        private IBuildPlan _plan;

        /// <summary>
        /// Resolves the IBuildPlan for this Instance.  The result is remembered
        /// for subsequent requests
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="policies"></param>
        /// <returns></returns>
        public IBuildPlan ResolveBuildPlan(Type pluginType, Policies policies)
        {
            lock (_buildLock)
            {
                return _plan ?? (_plan = buildPlan(pluginType, policies));
            }
        }

        /// <summary>
        /// Clears out any remembered IBuildPlan for this Instance
        /// </summary>
        public void ClearBuildPlan()
        {
            lock (_buildLock)
            {
                _plan = null;
            }
        }

        private string toDescription(Type pluginType)
        {
            var typeName = (pluginType ?? ReturnedType).GetFullName();

            if (HasExplicitName())
            {
                return "Instance of {0} ({1}) -- {2}".ToFormat(typeName, Name, Description);
            }

            return "Instance of {0} -- {1}".ToFormat(typeName, Description);
        }

        protected virtual IBuildPlan buildPlan(Type pluginType, Policies policies)
        {
            try
            {
                var builderSource = ToBuilder(pluginType, policies);
                var interceptors = determineInterceptors(pluginType, policies);

                return new BuildPlan(pluginType, this, builderSource, policies, interceptors);
            }
            catch (StructureMapException e)
            {
                e.Push("Attempting to create a BuildPlan for " + toDescription(pluginType));
                throw;
            }
            catch (Exception e)
            {
                throw new StructureMapBuildPlanException(
                    "Error while trying to create the BuildPlan for {0}.\nPlease check the inner exception".ToFormat(
                        toDescription(pluginType)), e);
            }
        }

        protected IEnumerable<IInterceptor> determineInterceptors(Type pluginType, Policies policies)
        {
            var interceptors =
                policies.Interceptors.SelectInterceptors(pluginType, this).Union(_interceptors);
            return interceptors;
        }

        /// <summary>
        /// Creates a hash that is unique for this Instance and PluginType combination
        /// </summary>
        /// <param name="pluginType"></param>
        /// <returns></returns>
        public int InstanceKey(Type pluginType)
        {
            unchecked
            {
                return ((_originalName != null ? _originalName.GetHashCode() : 0)*397) ^
                       (pluginType != null ? pluginType.AssemblyQualifiedName.GetHashCode() : 0);
            }
        }

        public ILifecycle DetermineLifecycle(ILifecycle parent)
        {
            return Lifecycle ?? parent ?? Lifecycles.Transient;
        }

        protected bool Equals(Instance other)
        {
            return string.Equals(_originalName, other._originalName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Instance) obj);
        }

        public override int GetHashCode()
        {
            return (_originalName != null ? _originalName.GetHashCode() : 0);
        }

        public Guid Id { get; private set; }
    }
}