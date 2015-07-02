using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Pipeline;
using StructureMap.Testing.Acceptance;

namespace StructureMap.Testing
{
    [TestFixture]
    public class BuildSession_tracks_the_parent_type_for_issue_272
    {
        private Type parentType;

        [SetUp]
        public void SetUp()
        {
            parentType = null;
        }

        [Test]
        public void for_basic_get_instance()
        {
            var container = new Container(x =>
            {
                x.For<ILoggerHolder>().Use<LoggerHolder>();

                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType));
            });

            container.GetInstance<ILoggerHolder>()
                .Logger.RootType.ShouldBe(typeof (LoggerHolder));
        }

        [Test]
        public void in_get_instance_by_name()
        {
            var container = new Container(x =>
            {
                x.For<ILoggerHolder>().Use<BuildSessionTarget>()
                    .Named("Red");

                x.For<ILoggerHolder>().Add<LoggerHolder>()
                    .Named("Blue");

                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType));
            });

            container.GetInstance<ILoggerHolder>("Red")
                .Logger.RootType.ShouldBe(typeof (BuildSessionTarget));

            container.GetInstance<ILoggerHolder>("Blue")
                .Logger.RootType.ShouldBe(typeof (LoggerHolder));

            container.GetInstance(typeof (ILoggerHolder), "Red")
                .As<ILoggerHolder>()
                .Logger.RootType.ShouldBe(typeof (BuildSessionTarget));

            container.GetInstance(typeof (ILoggerHolder), "Blue")
                .As<ILoggerHolder>()
                .Logger.RootType.ShouldBe(typeof (LoggerHolder));
        }

        [Test]
        public void from_within_get_all_instances()
        {
            var container = new Container(x =>
            {
                x.For<ILoggerHolder>().Use<BuildSessionTarget>()
                    .Named("Red");

                x.For<ILoggerHolder>().Add<LoggerHolder>()
                    .Named("Blue");

                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType)).AlwaysUnique();
            });

            var holders = container.GetAllInstances<ILoggerHolder>().ToArray();
            holders[0].Logger.RootType.ShouldBe(typeof (BuildSessionTarget));
            holders[1].Logger.RootType.ShouldBe(typeof (LoggerHolder));
        }

        [Test]
        public void get_instance_for_a_supplied_instance()
        {
            var container = new Container(x => { x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType)); });

            container.GetInstance<ILoggerHolder>(new SmartInstance<LoggerHolder>())
                .Logger.RootType
                .ShouldBe(typeof (LoggerHolder));
        }

        [Test]
        public void get_instance_with_args()
        {
            var container = new Container(x =>
            {
                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType));

                x.For<ILoggerHolder>().Use<WidgetLoggerHolder>();
            });

            var explicitArguments = new ExplicitArguments();
            explicitArguments.Set<IWidget>(new AWidget());
            container.GetInstance<ILoggerHolder>(explicitArguments)
                .Logger.RootType.ShouldBe(typeof (WidgetLoggerHolder));
        }


        [Test]
        public void get_instance_with_args_by_name()
        {
            var container = new Container(x =>
            {
                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType));

                x.For<ILoggerHolder>().Use<WidgetLoggerHolder>().Named("Foo");
            });

            var explicitArguments = new ExplicitArguments();
            explicitArguments.Set<IWidget>(new AWidget());
            container.GetInstance<ILoggerHolder>(explicitArguments, "Foo")
                .Logger.RootType.ShouldBe(typeof (WidgetLoggerHolder));
        }

        [Test]
        public void inside_a_func()
        {
            var container = new Container(x =>
            {
                x.For<FakeLogger>().Use(c => new FakeLogger(c.RootType));
                x.For<ILoggerHolder>().Use<LoggerHolder>();
            });

            container.GetInstance<LazyLoggerHolderHolder>()
                .Logger.RootType.ShouldBe(typeof (LoggerHolder));
        }
    }

    public interface ILoggerHolder
    {
        FakeLogger Logger { get; }
    }

    public class LazyLoggerHolderHolder
    {
        public LazyLoggerHolderHolder(Func<ILoggerHolder> func)
        {
            Logger = func().Logger;
        }

        public FakeLogger Logger { get; set; }
    }

    public class WidgetLoggerHolder : ILoggerHolder
    {
        private readonly IWidget _widget;
        private readonly FakeLogger _logger;

        public WidgetLoggerHolder(IWidget widget, FakeLogger logger)
        {
            _widget = widget;
            _logger = logger;
        }

        public FakeLogger Logger
        {
            get { return _logger; }
        }
    }

    public class BuildSessionTarget : ILoggerHolder
    {
        private readonly FakeLogger _logger;

        public BuildSessionTarget(FakeLogger logger)
        {
            _logger = logger;
        }

        public FakeLogger Logger
        {
            get { return _logger; }
        }
    }

    public class LoggerHolder : ILoggerHolder
    {
        private readonly FakeLogger _logger;

        public LoggerHolder(FakeLogger logger)
        {
            _logger = logger;
        }

        public FakeLogger Logger
        {
            get { return _logger; }
        }
    }

    public class FakeLogger
    {
        private readonly Type _rootType;

        public FakeLogger(Type rootType)
        {
            _rootType = rootType;
        }

        public Type RootType
        {
            get { return _rootType; }
        }
    }


    public static class LogManager
    {
        // Give me a Logger with the correctly
        // configured logging rules and sources
        // matching the type I'm passing in
        public static Logger ForType(Type type)
        {
            return new Logger(type);
        }
    }

    public class LoggerSample
    {
        public void contextual_logging()
        {
            // IContext.RootType is new for 3.1
            var container = new Container(_ =>
            {
                _.For<Logger>()
                    .Use(c => LogManager.ForType(c.RootType))
                    .AlwaysUnique();
            });

            // Resolve the logger for the type one level up
            container = new Container(_ =>
            {
                _.For<Logger>().Use(c => LogManager.ForType(c.ParentType))
                    .AlwaysUnique();
            });
        }
    }
}