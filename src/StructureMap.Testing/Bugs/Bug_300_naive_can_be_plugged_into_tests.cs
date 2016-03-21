using StructureMap.Pipeline;
using System;
using Xunit;

namespace StructureMap.Testing.Bugs
{
    public class Bug_300_naive_can_be_plugged_into_tests
    {
        public interface IInterface
        {
        }

        [Fact]
        public void throw_defensive_check_on_constructor_instance()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(
                () => { new ConstructorInstance(typeof(IInterface)); });
        }
    }
}