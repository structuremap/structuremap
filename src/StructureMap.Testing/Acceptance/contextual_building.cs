using System;
using NUnit.Framework;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class contextual_building
    {
        [Test]
        public void can_happily_use_the_parent_type()
        {
            var container = new Container(x => {
                x.For<Logger>().Use(c => new Logger(c.ParentType)).AlwaysUnique();
            });

            var top = container.GetInstance<LoggedClass1>();
            top.Logger.ParentType.ShouldBe(typeof (LoggedClass1));
            top.Child.Logger.ParentType.ShouldBe(typeof (LoggedClass2));
            top.Child.Child.Logger.ParentType.ShouldBe(typeof (LoggedClass3));

        }
    }


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