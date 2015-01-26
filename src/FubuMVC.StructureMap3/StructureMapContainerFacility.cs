using System;
using System.Collections.Generic;
using System.Diagnostics;
using Bottles;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.ObjectGraph;
using FubuMVC.Core.Runtime;
using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace FubuMVC.StructureMap3
{
    public class StructureMapContainerFacility : IContainerFacility, IServiceFactory
    {
        private readonly IContainer _container;
        private readonly Registry _registry;



        public StructureMapContainerFacility(IContainer container)
        {
            if (container == null) throw new ArgumentNullException("container");

            _container = container;
            _registry = new StructureMapFubuRegistry();
            _registry.For<IServiceFactory>().Use(this);

            _registration = (serviceType, def) =>
            {
                if (serviceType == typeof (Registry))
                {
                    var registry = def.Value as Registry;
                    if (registry != null)
                    {
                        _container.Configure(x => x.IncludeRegistry(registry));
                    }

                    if (def.Type.CanBeCastTo<Registry>() && def.Type.IsConcreteWithDefaultCtor())
                    {
                        registry = (Registry) Activator.CreateInstance(def.Type);
                        _container.Configure(x => x.IncludeRegistry(registry));
                    }

                    return;
                }

                if (def.Value == null)
                {
                    _registry.For(serviceType).Add(new ObjectDefInstance(def));
                }
                else
                {
                    _registry.For(serviceType).Add(new ObjectInstance(def.Value)
                    {
                        Name = def.Name
                    });
                }

                if (ServiceRegistry.ShouldBeSingleton(serviceType) || ServiceRegistry.ShouldBeSingleton(def.Type) || def.IsSingleton)
                {
                    _registry.For(serviceType).Singleton();
                }
            };
        }

        public IContainer Container
        {
            get { return _container; }
        }

        public virtual IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId)
        {
            return new NestedStructureMapContainerBehavior(_container, arguments, behaviorId);
        }

        public IServiceFactory BuildFactory()
        {
            _container.Configure(x => x.AddRegistry(_registry));

            _registration = (serviceType, def) =>
            {
                if (def.Value != null)
                {
                    _container.Configure(x => x.For(serviceType).Add(def.Value));
                }
                else
                {
                    _container.Configure(x => x.For(serviceType).Add(new ObjectDefInstance(def)));
                }

                
            };

            return this;
        }

        private Action<Type, ObjectDef> _registration; 

        public void Register(Type serviceType, ObjectDef def)
        {
            _registration(serviceType, def);
        }

        public void Shutdown()
        {
            _container.SafeDispose();
        }

        public T Get<T>()
        {
            return _container.GetInstance<T>();
        }

        public IEnumerable<T> GetAll<T>()
        {
            return _container.GetAllInstances<T>();
        }

        public static IContainer GetBasicFubuContainer()
        {
            return GetBasicFubuContainer(x => { });
        }

        public static IContainer GetBasicFubuContainer(Action<ConfigurationExpression> containerConfiguration)
        {
            var container = new Container(containerConfiguration);

            container.Configure(x => x.For<IHttpWriter>().Use(new NulloHttpWriter()));

            FubuApplication.For(() => new FubuRegistry()).StructureMap(container).Bootstrap();

            return container;
        }

        public void Dispose()
        {
            // DO NOTHING BECAUSE THIS CAUSED A STACKOVERFLOW TO DISPOSE THE CONTAINER HERE.
        }
    }

}