using System;
using StructureMap.Pipeline;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_563_explicit_arguments_failure
    {
        [Fact]
        public void do_not_blow_up()
        {
            var container = new Container(x =>
            {
                x.For(typeof(IMyService)).Use(typeof(MyService));
                //x.For<IMyService>().Use<MyService>(); // NOTE: this one works OK
            });

            var explicitArguments = new ExplicitArguments();

            // NOTE: Throws Unhandled Exception: 
            // StructureMap.StructureMapConfigurationException: No default instance or named instance Default for requested plugin type StructureMap.Reproducer.IMyService
            container.GetInstance<IMyService>(explicitArguments).ShouldNotBeNull();
        }

        public interface IMyService
        {

        }
        public class MyService : IMyService
        {

        }
    }
}