using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Interceptors;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ObjectBuilderTester
    {
        [Test]
        public void ObjectBuilder_should_throw_308_if_interception_fails()
        {
            try
            {
                var container = new Container(x =>
                {
                    x.For<Rule>().OnCreationForAll((c, r) => { throw new NotImplementedException(); })
                        .Use(() => new ColorRule("red"));
                });

                container.GetInstance<Rule>();

                Assert.Fail("Should have thrown error");
            }
            catch (StructureMapException e)
            {
                Assert.AreEqual(308, e.ErrorCode);
            }
        }

        [Test]
        public void should_apply_interception()
        {
            object comingAcross = null;

            var container = new Container(x =>
            {
                x.For<Rule>().OnCreationForAll((c, r) => comingAcross = r)
                    .Use(() => new ColorRule("red"));
            });

            container.GetInstance<Rule>().ShouldBeTheSameAs(comingAcross);
        }
    }
}