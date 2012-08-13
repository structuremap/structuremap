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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            graph = new PluginGraph();
            pipeline = new PipelineGraph(graph);
            library = new InterceptorLibrary();

            builder = new ObjectBuilder(pipeline, library);
        }

        #endregion

        private PluginGraph graph;
        private PipelineGraph pipeline;
        private InterceptorLibrary library;
        private ObjectBuilder builder;


        [Test]
        public void ObjectBuilder_should_throw_308_if_interception_fails()
        {
            try
            {
                var container = new Container(x =>
                {
                    x.ForRequestedType<Rule>().OnCreation((c, r) => { throw new NotImplementedException(); })
                        .TheDefault.Is.ConstructedBy(() => new ColorRule("red"));
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
                x.ForRequestedType<Rule>().OnCreation((c, r) => comingAcross = r)
                    .TheDefault.Is.ConstructedBy(() => new ColorRule("red"));
            });

            container.GetInstance<Rule>().ShouldBeTheSameAs(comingAcross);
        }
    }
}