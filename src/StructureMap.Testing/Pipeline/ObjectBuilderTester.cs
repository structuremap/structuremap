using StructureMap.Building;
using StructureMap.Testing.Widget;
using System;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class ObjectBuilderTester
    {
        [Fact]
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

        [Fact]
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