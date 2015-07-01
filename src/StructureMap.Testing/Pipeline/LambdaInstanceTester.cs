using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LambdaInstanceTester
    {
        [Test]
        public void Sad_path_inner_function_throws_exception_207_with_key_and_plugin_type()
        {
            var instance = new LambdaInstance<object>("throws", () => { throw new NotImplementedException(); });

            var ex = Exception<StructureMapBuildException>.ShouldBeThrownBy(() => {

                instance.Build<IWidget>(new StubBuildSession());
            });

            ex.Title.ShouldContain("'Lambda: throws'");
        }

        [Test]
        public void can_use_lambda_as_inline_dependency()
        {
            var container = new Container(x => {
                x.ForConcreteType<DecoratedGateway>().Configure
                    .Ctor<IGateway>().Is(c => new StubbedGateway());
            });

            container.GetInstance<DecoratedGateway>()
                .InnerGateway.ShouldBeOfType<StubbedGateway>();
        }
    }
}