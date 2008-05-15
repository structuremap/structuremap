using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing
{
    
    [TestFixture]
    public class BuildSessionTester
    {
        [SetUp]
        public void SetUp()
        {
        }

        private delegate void Action();
        private void assertActionThrowsErrorCode(int errorCode, Action action)
        {
            try
            {
                action();

                Assert.Fail("Should have thrown StructureMapException");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(errorCode, ex.ErrorCode);
            }
        }

        [Test]
        public void When_calling_CreateInstance_if_no_default_can_be_found_throw_202()
        {
            PipelineGraph graph = new PipelineGraph(new PluginGraph());

            assertActionThrowsErrorCode(202, delegate()
                                                 {
                                                     BuildSession session = new BuildSession(graph, null);
                                                     session.CreateInstance(typeof (IGateway));
                                                 });
        }

    }
}
