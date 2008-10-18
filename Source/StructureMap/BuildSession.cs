using System;
using System.Collections;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap
{
    public interface IContext
    {
        /// <summary>
        /// Gets a reference to the <see cref="BuildStack">BuildStack</see> for this build session
        /// </summary>
        BuildStack BuildStack { get; }

        /// <summary>
        /// The concrete type of the immediate parent object in the object graph
        /// </summary>
        Type ParentType { get; }

        /// <summary>
        /// Get the object of type T that is valid for this build session.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>();
    }

    public class BuildSession : IContext
    {
        private readonly BuildStack _buildStack = new BuildStack();
        private readonly InstanceCache _cache = new InstanceCache();
        private readonly Cache<Type, object> _defaults;
        private readonly InterceptorLibrary _interceptorLibrary;
        private readonly PipelineGraph _pipelineGraph;

        public BuildSession(PipelineGraph pipelineGraph, InterceptorLibrary interceptorLibrary)
        {
            _pipelineGraph = pipelineGraph;
            _interceptorLibrary = interceptorLibrary;

            _defaults = new Cache<Type, object>(t =>
            {
                Instance instance = _pipelineGraph.GetDefault(t);

                if (instance == null)
                {
                    throw new StructureMapException(202, t);
                }

                return CreateInstance(t, instance);
            });
        }

        public BuildSession(PluginGraph graph)
            : this(new PipelineGraph(graph), graph.InterceptorLibrary)
        {
        }

        public BuildSession() : this(new PluginGraph())
        {
        }


        protected PipelineGraph pipelineGraph
        {
            get { return _pipelineGraph; }
        }

        #region IContext Members

        public BuildStack BuildStack
        {
            get { return _buildStack; }
        }

        public Type ParentType
        {
            get { return _buildStack.Parent.ConcreteType; }
        }

        T IContext.GetInstance<T>()
        {
            return (T) CreateInstance(typeof (T));
        }

        #endregion

        public virtual object CreateInstance(Type pluginType, string name)
        {
            Instance instance = forType(pluginType).FindInstance(name);
            if (instance == null)
            {
                throw new StructureMapException(200, name, pluginType.FullName);
            }

            return CreateInstance(pluginType, instance);
        }

        public virtual object CreateInstance(Type pluginType, Instance instance)
        {
            object result = _cache.Get(pluginType, instance);

            if (result == null)
            {
                result = forType(pluginType).Build(this, instance);

                _cache.Set(pluginType, instance, result);
            }

            return result;
        }

        public virtual Array CreateInstanceArray(Type pluginType, Instance[] instances)
        {
            Array array;

            if (instances == null)
            {
                IList list = forType(pluginType).GetAllInstances(this);
                array = Array.CreateInstance(pluginType, list.Count);
                for (int i = 0; i < list.Count; i++)
                {
                    array.SetValue(list[i], i);
                }
            }
            else
            {
                array = Array.CreateInstance(pluginType, instances.Length);
                for (int i = 0; i < instances.Length; i++)
                {
                    Instance instance = instances[i];

                    object arrayValue = forType(pluginType).Build(this, instance);
                    array.SetValue(arrayValue, i);
                }
            }

            return array;
        }

        public virtual object CreateInstance(Type pluginType)
        {
            return _defaults.Retrieve(pluginType);
        }

        public virtual object ApplyInterception(Type pluginType, object actualValue)
        {
            if (actualValue == null) return null;
            return _interceptorLibrary.FindInterceptor(actualValue.GetType()).Process(actualValue);
        }

        public virtual void RegisterDefault(Type pluginType, object defaultObject)
        {
            _defaults.Store(pluginType, defaultObject);
        }


        private IInstanceFactory forType(Type pluginType)
        {
            return _pipelineGraph.ForType(pluginType);
        }
    }
}