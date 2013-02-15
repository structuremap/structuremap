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
        private readonly IObjectBuilder _builder;
        private readonly InstanceCache _cache = new InstanceCache();
        private readonly Cache<Type, Func<object>> _defaults;
        private readonly IPipelineGraph _pipelineGraph;
        private readonly object _locker = new object();

        [CLSCompliant(false)]
        protected BuildStack _buildStack = new BuildStack();

        public BuildSession(IPipelineGraph pipelineGraph, IObjectBuilder builder)
        {
            _builder = builder;
            _pipelineGraph = pipelineGraph;

            lock (this._locker)
            {
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
        }

        public static BuildSession ForPluginGraph(PluginGraph graph)
        {
            var pipeline = new PipelineGraph(graph);
            var builder = new ObjectBuilder(graph.InterceptorLibrary);

            return new BuildSession(pipeline, builder);
        }

        public static BuildSession Empty()
        {
            return ForPluginGraph(new PluginGraph());
        }

        protected IPipelineGraph pipelineGraph { get { return _pipelineGraph; } }

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
            return (T)CreateInstance(typeof(T));
        }

        public object GetInstance(Type pluginType)
        {
            return CreateInstance(pluginType);
        }

        public T GetInstance<T>(string name)
        {
            return (T)CreateInstance(typeof(T), name);
        }

        public object GetInstance(Type pluginType, string name)
        {
            return CreateInstance(pluginType, name);
        }

        BuildFrame IContext.Root { get { return _buildStack.Root; } }

        public virtual void RegisterDefault(Type pluginType, object defaultObject)
        {
            RegisterDefault(pluginType, () => defaultObject);
        }

        public T TryGetInstance<T>() where T : class
        {
            lock (this._locker)
            {
                if (_defaults.Has(typeof(T)))
                {
                    return (T)_defaults[typeof(T)]();
                }
            }

            return _pipelineGraph.HasDefaultForPluginType(typeof(T))
                       ? ((IContext)this).GetInstance<T>()
                       : null;
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return _pipelineGraph.HasInstance(typeof(T), name) ? ((IContext)this).GetInstance<T>(name) : null;
        }

        public object TryGetInstance(Type pluginType)
        {
            lock (this._locker)
            {
                if (_defaults.Has(pluginType))
                {
                    return _defaults[pluginType]();
                }
            }

            return _pipelineGraph.HasDefaultForPluginType(pluginType)
                       ? ((IContext)this).GetInstance(pluginType)
                       : null;
        }

        public object TryGetInstance(Type pluginType, string name)
        {
            return _pipelineGraph.HasInstance(pluginType, name) ? ((IContext)this).GetInstance(pluginType, name) : null;
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
            return _pipelineGraph.GetAllInstances(typeof(T)).Select(x => (T)CreateInstance(typeof(T), x));
        }

        protected void clearBuildStack()
        {
            _buildStack = new BuildStack();
        }

        public virtual object CreateInstance(Type pluginType, string name)
        {
            var instance = _pipelineGraph.FindInstance(pluginType, name);
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
                result = _builder.Resolve(pluginType, instance, this, _pipelineGraph);

                // TODO: HACK ATTACK!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                bool isUnique = _pipelineGraph.IsUnique(pluginType);
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
                instances = _pipelineGraph.GetAllInstances(pluginType).ToArray();
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
            var allInstances = _pipelineGraph.GetAllInstances(pluginType);
            return allInstances.Select(x => CreateInstance(pluginType, x));
        }

        public virtual object CreateInstance(Type pluginType)
        {
            lock (this._locker)
            {
                return _defaults[pluginType]();
            }
        }

        public virtual void RegisterDefault(Type pluginType, Func<object> creator)
        {
            lock (this._locker)
            {
                _defaults[pluginType] = creator;
            }
        }
    }
}
