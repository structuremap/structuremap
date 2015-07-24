using System;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace StructureMap
{
    /// <summary>
    /// A lightweight alternative to Registry for service registration
    /// </summary>
    public class ServiceRegistry 
    {
        /// <summary>
        /// The inner Registry of service registrations
        /// </summary>
        public readonly Registry Inner = new Registry();

        public static implicit operator Registry(ServiceRegistry registry)
        {
            return registry.Inner;
        }

        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public SmartInstance<TImplementation, TService> SetServiceIfNone<TService, TImplementation>() where TImplementation : TService
        {
            return Inner.For<TService>().UseIfNone<TImplementation>();
        }


        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        public void SetServiceIfNone(Type type, Instance instance)
        {
            Inner.For(type).Configure(x => x.Fallback = instance);
        }


        /// <summary>
        /// Sets the instanceault implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void SetServiceIfNone<TService>(TService value)
        {
            Inner.For<TService>().UseIfNone(value);
        }

        /// <summary>
        /// Sets the instance default implementation of a service if there is no
        /// previous registration
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        /// <returns></returns>
        public Instance SetServiceIfNone(Type interfaceType, Type concreteType)
        {
            var instance = new ConfiguredInstance(concreteType);

            Inner.For(interfaceType).Configure(family => family.Fallback = instance);

            return instance;
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        /// <returns></returns>
        public SmartInstance<TImplementation, TService> AddService<TService, TImplementation>() where TImplementation : TService
        {
            return Inner.For<TService>().Add<TImplementation>();
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        public Instance AddService<TService>(Type implementationType)
        {
            var instance = new ConfiguredInstance(implementationType);

            Inner.For<TService>().AddInstance(instance);

            return instance;
        }

        /// <summary>
        /// Registers a instanceault implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <typeparam name="TImplementation"></typeparam>
        public SmartInstance<TImplementation, TService> ReplaceService<TService, TImplementation>() where TImplementation : TService
        {
            Inner.For<TService>().ClearAll();
            return Inner.For<TService>().Use<TImplementation>();
        }

        /// <summary>
        /// Registers a instanceault implementation for a service.  Overwrites any existing
        /// registration
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void ReplaceService<TService>(TService value) where TService : class
        {
            Inner.For<TService>().ClearAll();
            Inner.For<TService>().Use(value);
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="value"></param>
        public void AddService<TService>(TService value) where TService : class
        {
            Inner.For<TService>().Add(value);
        }

        /// <summary>
        /// Removes any registrations for type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void AddService(Type type, Instance instance)
        {
            Inner.For(type).Add(instance);
        }

        /// <summary>
        /// Registers the concreteType against the interfaceType
        /// if the registration does not already include the concreteType 
        /// </summary>
        /// <param name="interfaceType"></param>
        /// <param name="concreteType"></param>
        public void ClearAll<T>()
        {
            Inner.For<T>().ClearAll();
        }

        /// <summary>
        /// Registers an *additional* implementation of a service.  Actual behavior varies by actual
        /// IoC container
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Instance"></param>
        public void FillType(Type interfaceType, Type concreteType)
        {
            Inner.AddType(interfaceType, concreteType);
        }
        /*

        public static bool ShouldBeSingleton(Type type)
        {
            if (type == null) return false;

            return type.Name.EndsWith("Cache") || type.HasAttribute<SingletonAttribute>();
        }
         */

        /// <summary>
        /// Replace any previous registrations with the supplied @default Instance
        /// </summary>
        /// <param name="type"></param>
        /// <param name="default"></param>
        public void ReplaceService(Type type, Instance @default)
        {
            Inner.For(type).ClearAll().Use(@default);
        }
    }


}