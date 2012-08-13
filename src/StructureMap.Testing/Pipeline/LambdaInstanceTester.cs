using System;
using NUnit.Framework;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LambdaInstanceTester
    {
        [Test]
        public void Sad_path_inner_function_throws_exception_207_with_key_and_plugin_type()
        {
            var instance = new LambdaInstance<object>(() => { throw new NotImplementedException(); });

            try
            {
                instance.Build(typeof (IWidget), new StubBuildSession());
                Assert.Fail("Should have thrown an exception");
            }
            catch (StructureMapException ex)
            {
                Assert.AreEqual(207, ex.ErrorCode);
            }
        }
    }
}