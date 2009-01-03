using System.Diagnostics;
using NUnit.Framework;

namespace StructureMap.Testing
{
    public interface IValidator
    {
    }

    public class Validator : IValidator
    {
        private readonly string _name;

        public Validator(string name)
        {
            _name = name;
        }

        public override string ToString()
        {
            return string.Format("Name: {0}", _name);
        }
    }

    public class ClassThatUsesValidators
    {
        private readonly IValidator[] _validators;

        public ClassThatUsesValidators(IValidator[] validators)
        {
            _validators = validators;
        }

        public void Write()
        {
            foreach (IValidator validator in _validators)
            {
                Debug.WriteLine(validator);
            }
        }
    }

    [TestFixture]
    public class ValidatorExamples
    {
        private Container container;

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.ForRequestedType<IValidator>().AddInstances(o =>
                {
                    o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Red").WithName("Red");
                    o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Blue").WithName("Blue");
                    o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Purple").WithName("Purple");
                    o.OfConcreteType<Validator>().WithCtorArg("name").EqualTo("Green").WithName("Green");
                });

                x.ForRequestedType<ClassThatUsesValidators>().AddInstances(o =>
                {
                    // Define an Instance of ClassThatUsesValidators that depends on AutoWiring
                    o.OfConcreteType<ClassThatUsesValidators>().WithName("WithAutoWiring");

                    // Define an Instance of ClassThatUsesValidators that overrides AutoWiring
                    o.OfConcreteType<ClassThatUsesValidators>().WithName("ExplicitArray")
                        .TheArrayOf<IValidator>().Contains(y =>
                        {
                            y.TheInstanceNamed("Red");
                            y.TheInstanceNamed("Green");
                        });
                });
            });
        }

        [Test]
        public void what_are_the_validators()
        {
            Debug.WriteLine("With Auto Wiring");
            container.GetInstance<ClassThatUsesValidators>("WithAutoWiring").Write();
            Debug.WriteLine("=================================");
            Debug.WriteLine("With Explicit Configuration");
            container.GetInstance<ClassThatUsesValidators>("ExplicitArray").Write();
        }
    }
}