using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget3;
using System;
using Xunit;

namespace StructureMap.Testing.Pipeline
{
    public class LambdaInstanceTester
    {
        [Fact]
        public void Sad_path_inner_function_throws_exception_207_with_key_and_plugin_type()
        {
            var instance = new LambdaInstance<object>("throws", () => { throw new NotImplementedException(); });

            var ex =
                Exception<StructureMapBuildException>.ShouldBeThrownBy(
                    () => { instance.Build<IWidget>(new StubBuildSession()); });

            ex.Title.ShouldContain("'Lambda: throws'");
        }

        [Fact]
        public void can_use_lambda_as_inline_dependency()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<DecoratedGateway>().Configure
                    .Ctor<IGateway>().Is(c => new StubbedGateway());
            });

            container.GetInstance<DecoratedGateway>()
                .InnerGateway.ShouldBeOfType<StubbedGateway>();
        }
    }
}