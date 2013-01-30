using System;
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
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            container = new Container(x =>
            {
                x.For<IValidator>().AddInstances(o =>
                {
                    o.Type<Validator>().Ctor<string>("name").EqualTo("Red").Named("Red");
                    o.Type<Validator>().Ctor<string>("name").EqualTo("Blue").Named("Blue");
                    o.Type<Validator>().Ctor<string>("name").EqualTo("Purple").Named("Purple");
                    o.Type<Validator>().Ctor<string>("name").EqualTo("Green").Named("Green");
                });

                x.For<ClassThatUsesValidators>().AddInstances(o =>
                {
                    // Define an Instance of ClassThatUsesValidators that depends on AutoWiring
                    o.Type<ClassThatUsesValidators>().Named("WithAutoWiring");

                    // Define an Instance of ClassThatUsesValidators that overrides AutoWiring
                    o.Type<ClassThatUsesValidators>().Named("ExplicitArray")
                        .TheArrayOf<IValidator>().Contains(y =>
                        {
                            y.TheInstanceNamed("Red");
                            y.TheInstanceNamed("Green");
                        });
                });
            });
        }

        #endregion

        private Container container;


        public class DataContext
        {
            private readonly Guid _id = Guid.NewGuid();

            public override string ToString()
            {
                return string.Format("Id: {0}", _id);
            }
        }

        public class Class1
        {
            private readonly DataContext _context;

            public Class1(DataContext context)
            {
                _context = context;
            }

            public override string ToString()
            {
                return string.Format("Class1 has Context: {0}", _context);
            }
        }

        public class Class2
        {
            private readonly Class1 _class1;
            private readonly DataContext _context;

            public Class2(Class1 class1, DataContext context)
            {
                _class1 = class1;
                _context = context;
            }

            public override string ToString()
            {
                return string.Format("Class2 has Context: {0}\n{1}", _context, _class1);
            }
        }

        public class Class3
        {
            private readonly Class2 _class2;
            private readonly DataContext _context;

            public Class3(Class2 class2, DataContext context)
            {
                _class2 = class2;
                _context = context;
            }

            public override string ToString()
            {
                return string.Format("Class3 has Context: {0}\n{1}", _context, _class2);
            }
        }

        [Test]
        public void demonstrate_session_identity()
        {
            var class3 = container.GetInstance<Class3>();
            Debug.WriteLine(class3);
        }

        [Test]
        public void demonstrate_session_identity_with_explicit_argument()
        {
            var context = new DataContext();
            Debug.WriteLine("The context being passed in is " + context);

            var class3 = container.With(context).GetInstance<Class3>();
            Debug.WriteLine(class3);
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