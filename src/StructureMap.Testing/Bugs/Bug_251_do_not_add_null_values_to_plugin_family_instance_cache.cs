using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_251_do_not_add_null_instances
    {
        [Test]
        public void should_not_add_null_instance()
        {
            var container = new Container(x => x.For<ITest>().Use<Test>());

            container.Model.Pipeline.Instances.GetAllInstances().Any(i => i == null).ShouldBeFalse();
            container.TryGetInstance<ITest>("test").ShouldBeNull();
            container.Model.Pipeline.Instances.GetAllInstances().Any(i => i == null).ShouldBeFalse();
        }

        public interface ITest
        {}

        public class Test : ITest
        { }
    }
}
