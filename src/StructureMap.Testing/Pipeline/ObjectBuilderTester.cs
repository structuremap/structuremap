using System;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ObjectBuilderTester
    {
        [Test]
        public void ObjectBuilder_should_throw_308_if_interception_fails()
        {
            Exception<StructureMapInterceptorException>.ShouldBeThrownBy(() =>
            {
                var container = new Container(x =>
                {
                    x.For<Rule>().OnCreationForAll("throwing up", (c, r) => { throw new NotImplementedException(); })
                        .Use(() => new ColorRule("red"));
                });

                container.GetInstance<Rule>();
            });
        }

        [Test]
        public void should_apply_interception()
        {
            object comingAcross = null;

            var container = new Container(x =>
            {
                x.For<Rule>().OnCreationForAll("coming across", (c, r) => comingAcross = r)
                    .Use(() => new ColorRule("red"));
            });

            var rule = container.GetInstance<Rule>();
            rule.ShouldBeTheSameAs(comingAcross);
        }
    }
}