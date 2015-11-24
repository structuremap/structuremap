using NUnit.Framework;

namespace StructureMap.Testing
{
    [TestFixture]
    public class PoliciesTester
    {
        [Test]
        public void CanBeAutoFilledIsFalse()
        {
            Policies.Default().CanBeAutoFilled(typeof (ClassWithPrimitiveConstructorArguments))
                .ShouldBeFalse();
        }

        [Test]
        public void CanBeAutoFilledIsTrue()
        {
            Policies.Default().CanBeAutoFilled(typeof (ClassWithAllNonSimpleConstructorArguments))
                .ShouldBeTrue();
        }

        [Test]
        public void CanBeAutoFilled_if_simple_property_has_default_value()
        {
            Policies.Default().CanBeAutoFilled(typeof (GuyWithName)).ShouldBeTrue();
        }


        [Test]
        public void cannot_be_auto_filled_with_no_contructors()
        {
            Policies.Default().CanBeAutoFilled(typeof (ClassWithNoConstructor))
                .ShouldBeFalse();
        }

        public class GuyWithName
        {
            public GuyWithName(string name = "Jim Croce")
            {
            }
        }

        public interface IAutomobile
        {
        }

        public interface IEngine
        {
        }

        public class ClassWithNoConstructor
        {
            private ClassWithNoConstructor()
            {
            }
        }

        public class ClassWithPrimitiveConstructorArguments : IAutomobile
        {
            private readonly string _breed;
            private readonly IEngine _engine;
            private readonly int _horsePower;

            public ClassWithPrimitiveConstructorArguments(int horsePower, string breed, IEngine engine)
            {
                _horsePower = horsePower;
                _breed = breed;
                _engine = engine;
            }
        }

        public class ClassWithAllNonSimpleConstructorArguments : IAutomobile
        {
            private readonly IEngine _engine;

            public ClassWithAllNonSimpleConstructorArguments(IEngine engine)
            {
                _engine = engine;
            }

            public IEngine Engine
            {
                get { return _engine; }
            }
        }
    }
}