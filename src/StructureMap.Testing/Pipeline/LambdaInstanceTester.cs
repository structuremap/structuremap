using System;
using NUnit.Framework;
using StructureMap.Building;
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

            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => {

                instance.Build<IWidget>(new StubBuildSession());
            });

            ex.Title.ShouldEqual("Failure at: \"Exception while trying to build 'Instance is created by Func<object> function:  System.Func`2[StructureMap.IContext,System.Object]', check the inner exception\"");
        }
    }
}