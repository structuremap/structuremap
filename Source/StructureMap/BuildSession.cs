using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Util;
using StructureMap.Construction;

namespace StructureMap
{
    public class BuildSession : IContext
    {
        private readonly ObjectBuilder _builder;
        private readonly InstanceCache _cache = new InstanceCache();
        private readonly Cache<Type, Func<object>> _defaults;
        private readonly PipelineGraph _pipelineGraph;

        [CLSCompliant(false)]
        protected BuildStack _buildStack = new BuildStack();

        public BuildSession(PipelineGraph pipelineGraph, InterceptorLibrary interceptorLibrary)
        {
            _builder = new ObjectBuilder(pipelineGraph, interceptorLibrary);
            _pipelineGraph = pipelineGraph;

            _defaults = new Cache<Type, Func<object>>(t =>
            {
                Instance instance = _pipelineGraph.GetDefault(t);

                if (instance == null)
                {
                    throw new StructureMapException(202, t);
                }

                return () => CreateInstance(t, instance);
            });
        }

        public BuildSession(PluginGraph graph)
            : this(new PipelineGraph(graph), graph.InterceptorLibrary)
        {
        }

        public BuildSession()
            : this(new PluginGraph())
        {
        }

        protected PipelineGraph pipelineGraph { get { return _pipelineGraph; } }

        #region IContext Members

        public string RequestedName { get; set; }

        public BuildStack BuildStack { get { return _buildStack; } }

        public Type ParentType
        {
            get
            {
                if (_buildStack.Parent != null) return _buildStack.Parent.ConcreteType;
                return null;
            }
        }

        public void BuildUp(object target)
        {
            Type pluggedType = target.GetType();
            IConfiguredInstance instance = _pipelineGraph.GetDefault(pluggedType) as IConfiguredInstance
                ?? new ConfiguredInstance(pluggedType);

            IInstanceBuilder builder = PluginCache.FindBuilder(pluggedType);
            var arguments = new Arguments(instance, this);
            builder.BuildUp(arguments, target);
        }

        public T GetInstance<T>()
        {
            return (T) CreateInstance(typeof (T));
        }

        public object GetInstance(Type pluginType)
        {
            return CreateInstance(pluginType);
        }

        public T GetInstance<T>(string name)
        {
            return (T) CreateInstance(typeof (T), name);
        }

        BuildFrame IContext.Root { get { return _buildStack.Root; } }

        public virtual void RegisterDefault(Type pluginType, object defaultObject)
        {
            RegisterDefault(pluginType, () => defaultObject);
        }

        public T TryGetInstance<T>() where T : class
        {
            if (_defaults.Has(typeof (T)))
            {
                return (T) _defaults[typeof (T)]();
            }

            return _pipelineGraph.HasDefaultForPluginType(typeof (T))
                       ? ((IContext) this).GetInstance<T>()
                       : null;
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return _pipelineGraph.HasInstance(typeof (T), name) ? ((IContext) this).GetInstance<T>(name) : null;
        }

        public IEnumerable<T> All<T>() where T : class
        {
            var list = new List<T>();
            _cache.Each<T>(list.Add);

            return list;
        }

        #endregion

        public IEnumerable<T> GetAllInstances<T>()
        {
            return (IEnumerable<T>) forType(typeof (T)).AllInstances.Select(x => (T)CreateInstance(typeof(T), x));
        }

        protected void clearBuildStack()
        {
            _buildStack = new BuildStack();
        }

        public virtual object CreateInstance(Type pluginType, string name)
        {
            Instance instance = forType(pluginType).FindInstance(name);
            if (instance == null)
            {
                throw new StructureMapException(200, name, pluginType.FullName);
            }

            return CreateInstance(pluginType, instance);
        }

        // This is where all Creation happens
        public virtual object CreateInstance(Type pluginType, Instance instance)
        {
            object result = _cache.Get(pluginType, instance);

            if (result == null)
            {
                result = _builder.Resolve(pluginType, instance, this);

                // TODO: HACK ATTACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                bool isUnique = forType(pluginType).Lifecycle is UniquePerRequestLifecycle;
                if (!isUnique)
                {
                    _cache.Set(pluginType, instance, result);
                }
            }

            return result;
        }

        [Obsolete("Move all of this into the new EnumerableInstance")]
        public virtual Array CreateInstanceArray(Type pluginType, Instance[] instances)
        {
            if (instances == null)
            {
                instances = forType(pluginType).AllInstances;
            }

            Array array = Array.CreateInstance(pluginType, instances.Length);
            for (int i = 0; i < instances.Length; i++)
            {
                Instance instance = instances[i];
                object arrayValue = CreateInstance(pluginType, instance);
                array.SetValue(arrayValue, i);
            }

            return array;
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            return forType(pluginType).AllInstances.Select(x => CreateInstance(pluginType, x));
        }

        public virtual object CreateInstance(Type pluginType)
        {
            return _defaults[pluginType]();
        }

        public virtual void RegisterDefault(Type pluginType, Func<object> creator)
        {
            _defaults[pluginType] = creator;
        }


        private IInstanceFactory forType(Type pluginType)
        {
            return _pipelineGraph.ForType(pluginType);
        }
    }
}
