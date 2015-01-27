using System;
using NUnit.Framework;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_300_naive_can_be_plugged_into_tests
    {
        public interface IInterface { }

        [Test]
        public void throw_defensive_check_on_constructor_instance()
        {
            Exception<ArgumentOutOfRangeException>.ShouldBeThrownBy(() =>
            {
                new ConstructorInstance(typeof (IInterface));
            });
        }
    }
}