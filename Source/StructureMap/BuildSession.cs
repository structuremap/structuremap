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

        /// <summary>
        /// Get the object of type T that is valid for this build session by name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T GetInstance<T>(string name);

        /// <summary>
        /// Gets the root "frame" of the object request
        /// </summary>
        BuildFrame Root { get; }

        /// <summary>
        /// The requested instance name of the object graph
        /// </summary>
        string RequestedName { get; }

        /// <summary>
        /// Register a default object for the given PluginType that will
        /// be used throughout the rest of the current object request
        /// </summary>
        /// <param name="pluginType"></param>
        /// <param name="defaultObject"></param>
        void RegisterDefault(Type pluginType, object defaultObject);

        /// <summary>
        /// Same as GetInstance, but can gracefully return null if 
        /// the Type does not already exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T TryGetInstance<T>() where T : class;

        /// <summary>
        /// Same as GetInstance(name), but can gracefully return null if 
        /// the Type and name does not already exist
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T TryGetInstance<T>(string name) where T : class;
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


        public string RequestedName { get; set; }

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

        public T GetInstance<T>(string name)
        {
            return (T) CreateInstance(typeof (T), name);
        }

        BuildFrame IContext.Root
        {
            get { return _buildStack.Root; }
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
            return _defaults[pluginType];
        }

        public virtual object ApplyInterception(Type pluginType, object actualValue)
        {
            if (actualValue == null) return null;
            return _interceptorLibrary.FindInterceptor(actualValue.GetType()).Process(actualValue, this);
        }

        public virtual void RegisterDefault(Type pluginType, object defaultObject)
        {
            _defaults[pluginType] = defaultObject;
        }

        public T TryGetInstance<T>() where T : class
        {
            if (_defaults.Has(typeof(T)))
            {
                return (T) _defaults[typeof (T)];
            }

            return _pipelineGraph.HasDefaultForPluginType(typeof (T))
                       ? ((IContext) this).GetInstance<T>()
                       : null;
        }

        public T TryGetInstance<T>(string name) where T : class
        {
            return _pipelineGraph.HasInstance(typeof (T), name) ? ((IContext) this).GetInstance<T>(name) : null;
        }


        private IInstanceFactory forType(Type pluginType)
        {
            return _pipelineGraph.ForType(pluginType);
        }
    }
}