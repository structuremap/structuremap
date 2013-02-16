using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap.Construction;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap
{
    public class BuildSession : IContext
    {
        private readonly InstanceCache _cache = new InstanceCache();
        private readonly Cache<Type, Func<object>> _defaults;
        private readonly object _locker = new object();
        private readonly IPipelineGraph _pipelineGraph;

        [CLSCompliant(false)] protected BuildStack _buildStack = new BuildStack();

        public BuildSession(IPipelineGraph pipelineGraph)
        {
            _pipelineGraph = pipelineGraph;

            lock (_locker)
            {
                _defaults = new Cache<Type, Func<object>>(t => {
                    Instance instance = _pipelineGraph.GetDefault(t);

                    if (instance == null)
                    {
                        throw new StructureMapException(202, t);
                    }

                    return () => CreateInstance(t, instance);
                });
            }
        }

        protected IPipelineGraph pipelineGraph
        {
            get { return _pipelineGraph; }
        }

        #region IContext Members

        public string RequestedName { get; set; }

        public BuildStack BuildStack
        {
            get { return _buildStack; }
        }

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

        public object GetInstance(Type pluginType, string name)
        {
            return CreateInstance(pluginType, name);
        }

        BuildFrame IContext.Root
        {
            get { return _buildStack.Root; }
        }

        public virtual void RegisterDefault(Type pluginType, object defaultObject)
        {
            RegisterDefault(pluginType, () => defaultObject);
        }

        public T TryGetInstance<T>() where T : class
        {
            lock (_locker)
            {
                if (_defaults.Has(typeof (T)))
                {
                    return (T) _defaults[typeof (T)]();
                }
            }

            return _pipelineGraph.HasDefaultForPluginType(typeof (T))
                       ? ((IContext) this).GetInstance<T>()
                       : null;
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return _pipelineGraph.HasInstance(typeof (T), name) ? ((IContext) this).GetInstance<T>(name) : null;
        }

        public object TryGetInstance(Type pluginType)
        {
            lock (_locker)
            {
                if (_defaults.Has(pluginType))
                {
                    return _defaults[pluginType]();
                }
            }

            return _pipelineGraph.HasDefaultForPluginType(pluginType)
                       ? ((IContext) this).GetInstance(pluginType)
                       : null;
        }

        public object TryGetInstance(Type pluginType, string name)
        {
            return _pipelineGraph.HasInstance(pluginType, name) ? ((IContext) this).GetInstance(pluginType, name) : null;
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
            return _pipelineGraph.GetAllInstances(typeof (T)).Select(x => (T) CreateInstance(typeof (T), x));
        }

        public IEnumerable<object> GetAllInstances(Type pluginType)
        {
            IEnumerable<Instance> allInstances = _pipelineGraph.GetAllInstances(pluginType);
            return allInstances.Select(x => CreateInstance(pluginType, x));
        }

        public static BuildSession ForPluginGraph(PluginGraph graph)
        {
            var pipeline = new PipelineGraph(graph);
            return new BuildSession(pipeline);
        }

        public static BuildSession Empty()
        {
            return ForPluginGraph(new PluginGraph());
        }

        protected void clearBuildStack()
        {
            _buildStack = new BuildStack();
        }

        public virtual object CreateInstance(Type pluginType, string name)
        {
            Instance instance = _pipelineGraph.FindInstance(pluginType, name);
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
                result = Resolve(pluginType, instance);

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

        public virtual object CreateInstance(Type pluginType)
        {
            lock (_locker)
            {
                return _defaults[pluginType]();
            }
        }

        public virtual void RegisterDefault(Type pluginType, Func<object> creator)
        {
            lock (_locker)
            {
                _defaults[pluginType] = creator;
            }
        }

        public object Resolve(Type pluginType, Instance instance)
        {
            IObjectCache cache = _pipelineGraph.FindCache(pluginType);
            lock (cache.Locker)
            {
                object returnValue = cache.Get(pluginType, instance);
                if (returnValue == null)
                {
                    returnValue = instance.Build(pluginType, this);
                    returnValue = ApplyInterception(pluginType, returnValue, instance);


                    cache.Set(pluginType, instance, returnValue);
                }

                return returnValue;
            }
        }

        public object ApplyInterception(Type pluginType, object actualValue, Instance instance)
        {
            if (actualValue == null) return null;

            try
            {
                return _pipelineGraph.FindInterceptor(actualValue.GetType()).Process(actualValue, this);
            }
            catch (Exception e)
            {
                throw new StructureMapException(308, e, instance.Name, actualValue.GetType());
            }
        }
    }
}