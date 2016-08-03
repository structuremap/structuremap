using Shouldly;
using System;
using System.Collections.Generic;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;
using Xunit;

namespace StructureMap.Testing.Acceptance
{
    public class nested_containers
    {
        // SAMPLE: nested-creation
        public interface IWorker
        {
            void DoWork();
        }

        public class Worker : IWorker, IDisposable
        {
            public void DoWork()
            {
                // do stuff!
            }

            public void Dispose()
            {
                // clean up
            }
        }

        [Fact]
        public void creating_a_nested_container()
        {
            // From an IContainer object
            var container = new Container(_ => { _.For<IWorker>().Use<Worker>(); });

            using (var nested = container.GetNestedContainer())
            {
                // This object is disposed when the nested container
                // is disposed
                var worker = nested.GetInstance<IWorker>();
                worker.DoWork();
            }
        }

        // ENDSAMPLE

        // SAMPLE: nested-singletons
        [Fact]
        public void nested_container_usage_of_singletons()
        {
            var container = new Container(_ => { _.ForSingletonOf<IColorCache>().Use<ColorCache>(); });

            var singleton = container.GetInstance<IColorCache>();

            // SingletonThing's are resolved from the parent container
            using (var nested = container.GetNestedContainer())
            {
                nested.GetInstance<IColorCache>()
                    .ShouldBeTheSameAs(singleton);
            }
        }

        // ENDSAMPLE

        // SAMPLE: nested-transients
        [Fact]
        public void nested_container_behavior_of_transients()
        {
            // "Transient" is the default lifecycle
            // in StructureMap
            var container = new Container(_ => { _.For<IColor>().Use<Green>(); });

            // In a normal Container, a "transient" lifecycle
            // Instance will be built up once in every request
            // to the Container
            container.GetInstance<IColor>()
                .ShouldNotBeTheSameAs(container.GetInstance<IColor>());

            // From a nested container, the "transient" lifecycle
            // is tracked to the nested container
            using (var nested = container.GetNestedContainer())
            {
                nested.GetInstance<IColor>()
                    .ShouldBeTheSameAs(nested.GetInstance<IColor>());

                // One more time
                nested.GetInstance<IColor>()
                    .ShouldBeTheSameAs(nested.GetInstance<IColor>());
            }
        }

        // ENDSAMPLE

        // SAMPLE: nested-profiles
        [Fact]
        public void nested_container_from_profile_container()
        {
            var container = new Container(x =>
            {
                x.For<IColor>().Use<Red>();

                x.Profile("Blue", _ => _.For<IColor>().Use<Blue>());
                x.Profile("Green", _ => _.For<IColor>().Use<Green>());
            });

            using (var nested = container.GetProfile("Blue").GetNestedContainer())
            {
                nested.GetInstance<IColor>().ShouldBeOfType<Blue>();
            }

            using (var nested = container.GetNestedContainer("Green"))
            {
                nested.GetInstance<IColor>().ShouldBeOfType<Green>();
            }
        }

        // ENDSAMPLE

        // SAMPLE: nested-disposal
        [Fact]
        public void nested_container_disposal()
        {
            var container = new Container(_ =>
            {
                // A SingletonThing scoped service
                _.ForSingletonOf<IColorCache>().Use<ColorCache>();

                // A transient scoped service
                _.For<IColor>().Use<Green>();

                // An AlwaysUnique scoped service
                _.For<Purple>().AlwaysUnique();
            });

            ColorCache singleton = null;
            Green nestedGreen = null;
            Blue nestedBlue = null;
            Purple nestedPurple = null;

            using (var nested = container.GetNestedContainer())
            {
                // SingletonThing's are really built by the parent
                singleton = nested.GetInstance<IColorCache>()
                    .ShouldBeOfType<ColorCache>();

                nestedGreen = nested.GetInstance<IColor>()
                    .ShouldBeOfType<Green>();

                nestedBlue = nested.GetInstance<Blue>();

                nestedPurple = nested.GetInstance<Purple>();
            }

            // Transients created by the Nested Container
            // are disposed
            nestedGreen.WasDisposed.ShouldBeTrue();
            nestedBlue.WasDisposed.ShouldBeTrue();

            // Unique's created by the Nested Container
            // are disposed
            nestedPurple.WasDisposed.ShouldBeTrue();

            // NOT disposed because it's owned by
            // the parent container
            singleton.WasDisposed.ShouldBeFalse();
        }

        // ENDSAMPLE

        // SAMPLE: nested-overriding
        [Fact]
        public void overriding_services_in_a_nested_container()
        {
            var container = new Container(_ =>
            {
                _.For<IHttpRequest>().Use<StandInHttpRequest>();
                _.For<IHttpResponse>().Use<StandInHttpResponse>();
            });

            var request = new HttpRequest();
            var response = new HttpResponse();

            using (var nested = container.GetNestedContainer())
            {
                // Override the HTTP request and response for this
                // nested container
                nested.Configure(_ =>
                {
                    _.For<IHttpRequest>().Use(request);
                    _.For<IHttpResponse>().Use(response);
                });

                var handler = nested.GetInstance<HttpRequestHandler>();
                handler.Request.ShouldBeTheSameAs(request);
                handler.Response.ShouldBeTheSameAs(response);
            }

            // Outside the nested container, we still have the original
            // registrations
            container.GetInstance<IHttpRequest>()
                .ShouldBeOfType<StandInHttpRequest>();

            container.GetInstance<IHttpResponse>()
                .ShouldBeOfType<StandInHttpResponse>();
        }

        // ENDSAMPLE

        // SAMPLE: nested-func-lazy-and-container-resolution
        public class Foo
        {
        }

        public class FooHolder
        {
            public IContainer Container { get; set; }
            public Func<Foo> Func { get; set; }
            public Lazy<Foo> Lazy { get; set; }

            public FooHolder(IContainer container, Func<Foo> func, Lazy<Foo> lazy)
            {
                Container = container;
                Func = func;
                Lazy = lazy;
            }
        }

        [Fact]
        public void service_location_and_container_resolution_inside_nested_containers()
        {
            var container = new Container();

            using (var nested = container.GetNestedContainer())
            {
                var holder = nested.GetInstance<FooHolder>();

                // The injected IContainer is the nested container
                holder.Container.ShouldBeTheSameAs(nested);

                // Func<T> and Lazy<T> values will be built by
                // the nested container w/ the nested container
                // scoping
                var nestedFoo = nested.GetInstance<Foo>();

                holder.Func().ShouldBeTheSameAs(nestedFoo);
                holder.Lazy.Value.ShouldBeTheSameAs(nestedFoo);
            }
        }

        // ENDSAMPLE

        [Fact]
        public void always_unique_disposal()
        {
            var container = new Container(_ =>
            {
                _.For<Blue>().AlwaysUnique();
            });

            Blue nestedBlue1;
            Blue nestedBlue2;
            using (var nested = container.GetNestedContainer())
            {
                nestedBlue1 = nested.GetInstance<Blue>();
                nestedBlue2 = nested.GetInstance<Blue>();

                nestedBlue1.ShouldNotBeTheSameAs(nestedBlue2);
            }

            nestedBlue1.WasDisposed.ShouldBeTrue();
            nestedBlue2.WasDisposed.ShouldBeTrue();
        }

        [Fact]
        public void build_nested_container_with_defaults()
        {
            var container = new Container(_ =>
            {
                _.For<Blue>().Use<Blue>();
                _.For<IWidget>().Use<DefaultWidget>();
                _.For<Rule>().Use<ARule>();
            });

            var nestedWidget = new AWidget();
            var nestedBlue = new Blue();
            var arguments = new TypeArguments().Set(nestedBlue).Set<IWidget>(nestedWidget);

            var nested = container.GetNestedContainer(arguments);

            nested.GetInstance<IWidget>().ShouldBeTheSameAs(nestedWidget);
            nested.GetInstance<Blue>().ShouldBeTheSameAs(nestedBlue);

            // fallback to the parent w/ no defaults
            nested.GetInstance<Rule>().ShouldBeOfType<ARule>();

            // main container is not corrupted by the nested registrations
            container.GetInstance<IWidget>().ShouldBeOfType<DefaultWidget>();
        }
    }

    // SAMPLE: nested-colors
    public interface IColor
    {
    }

    public class Red : IColor
    {
    }

    public class Purple : Blue { }

    public class Blue : IColor, IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public class Green : IColor, IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    public interface IColorCache
    {
    }

    public class ColorCache : IColorCache, IDisposable
    {
        public bool WasDisposed;

        public void Dispose()
        {
            WasDisposed = true;
        }
    }

    // ENDSAMPLE

    // SAMPLE: nested-http
    public interface IHttpRequest
    {
    }

    public interface IHttpResponse
    {
    }

    public class HttpRequest : IHttpRequest
    {
    }

    public class HttpResponse : IHttpResponse
    {
    }

    public class StandInHttpRequest : IHttpRequest
    {
    }

    public class StandInHttpResponse : IHttpResponse
    {
    }

    public class HttpRequestHandler
    {
        private readonly IHttpRequest _request;
        private readonly IHttpResponse _response;

        public HttpRequestHandler(IHttpRequest request, IHttpResponse response)
        {
            _request = request;
            _response = response;
        }

        public IHttpRequest Request
        {
            get { return _request; }
        }

        public IHttpResponse Response
        {
            get { return _response; }
        }
    }

    // ENDSAMPLE

    public interface IUnitOfWork
    {
    }

    public class UnitOfWork : IUnitOfWork
    {
    }

    public class HandlerA
    {
        public HandlerA(IUnitOfWork uow)
        {
        }
    }

    public class HandlerB
    {
        public HandlerB(IUnitOfWork uow)
        {
        }
    }

    public class HandlerC
    {
        public HandlerC(IUnitOfWork uow)
        {
        }
    }
}