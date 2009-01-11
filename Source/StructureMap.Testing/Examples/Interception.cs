using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Interceptors;
using StructureMap;

namespace StructureMap.Testing.Examples
{
    public interface IConnectionListener
    {
        void StartConnection();
    }

    public class ClassThatNeedsSomeBootstrapping : IConnectionListener
    {
        public void Start()
        {
            
        }

        public void Connect(IConnectionPoint connection)
        {
            
        }

        public void StartConnection()
        {
            throw new NotImplementedException();
        }
    }

    public class LoggingDecorator : IConnectionListener
    {
        private readonly IConnectionListener _inner;

        public LoggingDecorator(IConnectionListener inner)
        {
            _inner = inner;
        }

        public IConnectionListener Inner
        {
            get { return _inner; }
        }

        public void StartConnection()
        {

        }
    }

    public class InterceptionRegistry : Registry
    {
        public InterceptionRegistry()
        {
            // Perform an Action<T> upon the object of type T 
            // just created before it is returned to the caller
            ForRequestedType<ClassThatNeedsSomeBootstrapping>().TheDefault.Is
                .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
                .OnCreation(x => x.Start());
                
            // or...

            // You can also register an Action<IContext, T> to get access
            // to all the services and capabilities of the BuildSession
            ForRequestedType<ClassThatNeedsSomeBootstrapping>().TheDefault.Is
                .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
                .OnCreation((context, x) =>
                {
                    var connection = context.GetInstance<IConnectionPoint>();
                    x.Connect(connection);
                });


            ForRequestedType<IConnectionListener>().TheDefault.Is
                .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
                .EnrichWith(x => new LoggingDecorator(x));

            ForRequestedType<IConnectionListener>().TheDefault.Is
                .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
                .EnrichWith((context, x) =>
                {
                    var connection = context.GetInstance<IConnectionPoint>();
                    x.Connect(connection);

                    return new LoggingDecorator(x);
                });


            ForRequestedType<IConnectionListener>().TheDefault.Is
                .OfConcreteType<ClassThatNeedsSomeBootstrapping>()
                .InterceptWith(new CustomInterceptor());



            // Place the Interception at the PluginType level
            ForRequestedType<IConnectionListener>()
                .OnCreation(x => x.StartConnection())     // OnCreation
                .EnrichWith(x => new LoggingDecorator(x)) // Enrich
                .InterceptWith(new CustomInterceptor())   // Custom Interceptor


                .TheDefaultIsConcreteType<ClassThatNeedsSomeBootstrapping>();

        }
    }

    [TestFixture, Explicit]
    public class InterceptionRegistryInAction
    {
        [Test]
        public void see_the_enrichment_with_a_decorator_in_action()
        {
            var container = new Container(new InterceptionRegistry());
            container.GetInstance<IConnectionListener>()
                .ShouldBeOfType<LoggingDecorator>()
                .Inner.ShouldBeOfType<ClassThatNeedsSomeBootstrapping>();
        }
    }

    public class CustomInterceptor : InstanceInterceptor
    {
        public object Process(object target, IContext context)
        {
            // manipulate the target object and return a wrapped version
            return wrapTarget(target);
        }

        private object wrapTarget(object target)
        {
            throw new NotImplementedException();
        }
    }


    

    public interface IEventListener<T>
    {
        void ProcessEvent(T @event);
    }

    public interface IEventAggregator
    {
        void RegisterListener<T>(IEventListener<T> listener);
        void PublishEvent<T>(T @event);
    }

    

    public class ListenerInterceptor : TypeInterceptor
    {
        public object Process(object target, IContext context)
        {
            // Assuming that "target" is an implementation of IEventListener<T>,
            // we'll do a little bit of generics sleight of hand
            // to register "target" with IEventAggregator
            var eventType = target.GetType().FindInterfaceThatCloses(typeof (IEventListener<>)).GetGenericArguments()[0];
            var type = typeof (Registration<>).MakeGenericType(eventType);
            Registration registration = (Registration) Activator.CreateInstance(type);
            registration.RegisterListener(context, target);
            
            // we didn't change the target object, so just return it
            return target;
        }

        public bool MatchesType(Type type)
        {
            // ImplementsInterfaceTemplate is an Extension method in the
            // StructureMap namespace that basically says:
            // does this type implement any closed type of the open template type?
            return type.ImplementsInterfaceTemplate(typeof (IEventListener<>));
        }

        // The inner type and interface is just a little trick to
        // grease the generic wheels
        public interface Registration
        {
            void RegisterListener(IContext context, object listener);
        }

        public class Registration<T> : Registration
        {
            public void RegisterListener(IContext context, object listener)
            {
                var aggregator = context.GetInstance<IEventAggregator>();
                aggregator.RegisterListener<T>((IEventListener<T>) listener);
            }
        }
    }

    public class ListeningRegistry : Registry
    {
        public ListeningRegistry()
        {
            RegisterInterceptor(new ListenerInterceptor());
        }
    }
}
