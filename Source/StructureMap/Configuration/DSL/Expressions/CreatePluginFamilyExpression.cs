using System;
using System.Collections.Generic;
using StructureMap.Attributes;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;

namespace StructureMap.Configuration.DSL.Expressions
{
    /// <summary>
    /// Expression Builder that has grammars for defining policies at the 
    /// PluginType level
    /// </summary>
    public class CreatePluginFamilyExpression<PLUGINTYPE>
    {
        private readonly List<Action<PluginFamily>> _alterations = new List<Action<PluginFamily>>();
        private readonly List<Action<PluginGraph>> _children = new List<Action<PluginGraph>>();
        private readonly Type _pluginType;

        public CreatePluginFamilyExpression(Registry registry)
        {
            _pluginType = typeof (PLUGINTYPE);

            registry.addExpression(graph =>
            {
                PluginFamily family = graph.FindFamily(_pluginType);

                _children.ForEach(action => action(graph));
                _alterations.ForEach(action => action(family));
            });
        }

        /// <summary>
        /// Define the Default Instance for this PluginType
        /// </summary>
        public IsExpression<PLUGINTYPE> TheDefault
        {
            get { return new InstanceExpression<PLUGINTYPE>(i => registerDefault(i)); }
        }

        public InstanceExpression<PLUGINTYPE> MissingNamedInstanceIs
        {
            get
            {
                return new InstanceExpression<PLUGINTYPE>(i => _alterations.Add(family => family.MissingInstance = i));
            }
        }

        /// <summary>
        /// Add multiple Instance's to this PluginType
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstances(Action<IInstanceExpression<PLUGINTYPE>> action)
        {
            var list = new List<Instance>();

            var child = new InstanceExpression<PLUGINTYPE>(i => list.Add(i));
            action(child);

            return alterAndContinue(family =>
            {
                foreach (Instance instance in list)
                {
                    family.AddInstance(instance);
                }
            });
        }


        private CreatePluginFamilyExpression<PLUGINTYPE> alterAndContinue(Action<PluginFamily> action)
        {
            _alterations.Add(action);
            return this;
        }

        /// <summary>
        /// Convenience method that sets the default concrete type of the PluginType.  Type T
        /// can only accept types that do not have any primitive constructor arguments.
        /// StructureMap has to know how to construct all of the constructor argument types.
        /// </summary>
        /// <typeparam name="CONCRETETYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> TheDefaultIsConcreteType<CONCRETETYPE>()
            where CONCRETETYPE : PLUGINTYPE
        {
            Type concreteType = typeof (CONCRETETYPE);

            ExpressionValidator.ValidatePluggabilityOf(concreteType).IntoPluginType(_pluginType);

            if (!PluginCache.GetPlugin(concreteType).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }
            
            return alterAndContinue(family =>
            {
                ConfiguredInstance instance =
                    new ConfiguredInstance(concreteType).WithName(concreteType.AssemblyQualifiedName);
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        /// <summary>
        /// Sets the object creation of the instances of the PluginType.  For example:  PerRequest,
        /// Singleton, ThreadLocal, HttpContext, or Hybrid
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> CacheBy(InstanceScope scope)
        {
            return alterAndContinue(family => family.SetScopeTo(scope));
        }

        /// <summary>
        /// Convenience method to mark a PluginFamily as a Singleton
        /// </summary>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AsSingletons()
        {
            _alterations.Add(family => family.SetScopeTo(InstanceScope.Singleton));
            return this;
        }

        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(Action<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<object, object> function = target =>
                    {
                        handler((PLUGINTYPE) target);
                        return target;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), (c, o) =>
                    {
                        handler((PLUGINTYPE) o);
                        return o;
                    });

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> InterceptWith(InstanceInterceptor interceptor)
        {
            _children.Add(
                graph =>
                {
                    var typeInterceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), (c, o) => interceptor.Process(o, c));
                    graph.InterceptorLibrary.AddInterceptor(typeInterceptor);
                });

            return this;
        }

        /// <summary>
        /// Register an Action to run against any object of this PluginType immediately after
        /// it is created, but before the new object is passed back to the caller
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> OnCreation(Action<IContext, PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (c, o) =>
                    {
                        handler(c, (PLUGINTYPE)o);
                        return o;
                    };

                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), function);

                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(EnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    Func<IContext, object, object> function = (context, target) => handler((PLUGINTYPE) target);

                    var interceptor = new PluginTypeInterceptor(typeof (PLUGINTYPE), function);
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Register a Func to run against any object of this PluginType immediately after it is created,
        /// but before the new object is passed back to the caller.  Unlike <see cref="OnCreation(Action{IContext,PLUGINTYPE})">OnCreation()</see>,
        /// EnrichWith() gives the the ability to return a different object.  Use this method for runtime AOP
        /// scenarios or to return a decorator.
        /// </summary>
        /// <param name="handler"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> EnrichWith(ContextEnrichmentHandler<PLUGINTYPE> handler)
        {
            _children.Add(
                graph =>
                {
                    var interceptor = new PluginTypeInterceptor(typeof(PLUGINTYPE), (c, o) => handler(c, (PLUGINTYPE)o));
                    graph.InterceptorLibrary.AddInterceptor(interceptor);
                });

            return this;
        }

        /// <summary>
        /// Shortcut method to add an additional Instance to this Plugin Type
        /// as just a Concrete Type.  This will only work if the Concrete Type
        /// has no primitive constructor or mandatory Setter arguments.
        /// </summary>
        /// <typeparam name="PLUGGEDTYPE"></typeparam>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddConcreteType<PLUGGEDTYPE>()
        {
            if (!PluginCache.GetPlugin(typeof(PLUGGEDTYPE)).CanBeAutoFilled)
            {
                throw new StructureMapException(231);
            }

            _alterations.Add(family =>
            {
                string name = PluginCache.GetPlugin(typeof (PLUGGEDTYPE)).ConcreteKey;
                SmartInstance<PLUGGEDTYPE> instance = new SmartInstance<PLUGGEDTYPE>().WithName(name);
                family.AddInstance(instance);
            });

            return this;
        }

        /// <summary>
        /// Registers an IBuildInterceptor for this Plugin Type that executes before
        /// any object of this PluginType is created.  IBuildInterceptor's can be
        /// used to create a custom scope
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> InterceptConstructionWith(IBuildInterceptor interceptor)
        {
            _alterations.Add(family => family.AddInterceptor(interceptor));
            return this;
        }

        /// <summary>
        /// Registers an IBuildInterceptor for this Plugin Type that executes before
        /// any object of this PluginType is created.  IBuildInterceptor's can be
        /// used to create a custom scope
        /// </summary>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> BuildPolicyIs(IBuildInterceptor interceptor)
        {
            _alterations.Add(family => family.AddInterceptor(interceptor));
            return this;
        }

        /// <summary>
        /// Largely deprecated and unnecessary with the ability to add Xml configuration files
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public CreatePluginFamilyExpression<PLUGINTYPE> AddInstancesFrom(MementoSource source)
        {
            _alterations.Add(family => family.AddMementoSource(source));

            return this;
        }

        private void registerDefault(Instance instance)
        {
            _alterations.Add(family =>
            {
                family.AddInstance(instance);
                family.DefaultInstanceKey = instance.Name;
            });
        }

        public CreatePluginFamilyExpression<PLUGINTYPE> AlwaysUnique()
        {
            return InterceptConstructionWith(new UniquePerRequestInterceptor());
        }
    }
}