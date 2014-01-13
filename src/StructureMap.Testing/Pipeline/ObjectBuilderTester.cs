using System;
using System.Diagnostics;
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
            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => {
                var container = new Container(x =>
                {
                    x.For<Rule>().OnCreationForAll((c, r) => { throw new NotImplementedException(); })
                        .Use(() => new ColorRule("red"));
                });

                container.GetInstance<Rule>();

            });

            ex.Title.ShouldContain("Failure at: \"A configured instance interceptor");

        }

        [Test]
        public void should_apply_interception()
        {
            object comingAcross = null;

            var container = new Container(x => {
                x.For<Rule>().OnCreationForAll((c, r) => comingAcross = r)
                    .Use(() => new ColorRule("red"));
            });

            container.GetInstance<Rule>().ShouldBeTheSameAs(comingAcross);
        }
    }
}