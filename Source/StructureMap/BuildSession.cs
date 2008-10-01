using System;
using System.Collections;
using System.Collections.Generic;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Util;

namespace StructureMap
{
    public interface IContext
    {
        T GetInstance<T>();


        BuildStack BuildStack { get; }
        Type ParentType { get; }
    }

    public class BuildSession : IContext
    {
        private readonly InterceptorLibrary _interceptorLibrary;
        private readonly PipelineGraph _pipelineGraph;
        private readonly InstanceCache _cache = new InstanceCache();
        private readonly Cache<Type, object> _defaults;
        private readonly BuildStack _buildStack = new BuildStack();

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

        public BuildStack BuildStack
        {
            get { return _buildStack; }
        }

        public Type ParentType
        {
            get { return _buildStack.Parent.ConcreteType; }
        }


        private IInstanceFactory forType(Type pluginType)
        {
            return _pipelineGraph.ForType(pluginType);
        }

        T IContext.GetInstance<T>()
        {
            return (T) CreateInstance(typeof (T));
        }
    }
}