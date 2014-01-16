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
                    x.For<Rule>().OnCreationForAll("throwing up", (c, r) => { throw new NotImplementedException(); })
                        .Use(() => new ColorRule("red"));
                });

                container.GetInstance<Rule>();

            });

            ex.Title.ShouldContain("Failure at: \"Invoke(value(StructureMap.Building.Interception.InterceptorFactory+<>c__DisplayClass2`1[StructureMap.Testing.Widget.Rule]).action, IBuildSession, Rule)\"");

        }

        [Test]
        public void should_apply_interception()
        {
            object comingAcross = null;

            var container = new Container(x => {
                x.For<Rule>().OnCreationForAll("coming across", (c, r) => comingAcross = r)
                    .Use(() => new ColorRule("red"));
            });

            var rule = container.GetInstance<Rule>();
            rule.ShouldBeTheSameAs(comingAcross);
        }
    }
}