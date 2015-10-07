using System;
using System.Linq;
using NUnit.Framework;
using Shouldly;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Acceptance
{
    
    [TestFixture]
    public class contextual_building
    {

        // SAMPLE: Logger-contextual-building
        [Test]
        public void can_happily_use_the_parent_type()
        {
            var container = new Container(x =>
            {
                // AlwaysUnique() is important so that every object created will get
                // their own Logger instead of sharing whichever one is created first
                x.For<Logger>().Use(c => LoggerFactory.LoggerFor(c.ParentType)).AlwaysUnique();
            });

            var top = container.GetInstance<LoggedClass1>();
            top.Logger.ParentType.ShouldBe(typeof (LoggedClass1));
            top.Child.Logger.ParentType.ShouldBe(typeof (LoggedClass2));
            top.Child.Child.Logger.ParentType.ShouldBe(typeof (LoggedClass3));
        }
        // ENDSAMPLE

        // SAMPLE: LoggerConvention
        public class LoggerConvention : ConfiguredInstancePolicy
        {
            protected override void apply(Type pluginType, IConfiguredInstance instance)
            {
                instance.Constructor
                    .GetParameters()
                    .Where(param => param.ParameterType == typeof(Logger))
                    .Each(param => instance.Dependencies.Add(LoggerFactory.LoggerFor(instance.PluggedType)));
            }
        }

        [Test]
        public void use_logger_convention()
        {
            var container = new Container(_ =>
            {
                _.Policies.Add<LoggerConvention>();
            });

            var top = container.GetInstance<LoggedClass1>();
            top.Logger.ParentType.ShouldBe(typeof(LoggedClass1));
            top.Child.Logger.ParentType.ShouldBe(typeof(LoggedClass2));
            top.Child.Child.Logger.ParentType.ShouldBe(typeof(LoggedClass3));
        }
        // ENDSAMPLE
    }
    








    // SAMPLE: LoggerFactory
    public static class LoggerFactory
    {
        public static Logger LoggerFor(Type type)
        {
            return new Logger(type);
        }
    }
    // ENDSAMPLE


    public class LoggedClass1
    {
        private readonly LoggedClass2 _child;
        private readonly Logger _logger;

        public LoggedClass1(LoggedClass2 child, Logger logger)
        {
            _child = child;
            _logger = logger;
        }

        public LoggedClass2 Child
        {
            get { return _child; }
        }

        public Logger Logger
        {
            get { return _logger; }
        }
    }

    public class LoggedClass2
    {
        private readonly LoggedClass3 _child;
        private readonly Logger _logger;

        public LoggedClass2(LoggedClass3 child, Logger logger)
        {
            _child = child;
            _logger = logger;
        }

        public LoggedClass3 Child
        {
            get { return _child; }
        }

        public Logger Logger
        {
            get { return _logger; }
        }
    }

    public class LoggedClass3
    {
        private readonly Logger _logger;

        public LoggedClass3(Logger logger)
        {
            _logger = logger;
        }

        public Logger Logger
        {
            get { return _logger; }
        }
    }

    public class Logger
    {
        private readonly Type _parentType;

        public Logger(Type parentType)
        {
            _parentType = parentType;
        }

        public Type ParentType
        {
            get { return _parentType; }
        }
    }
}