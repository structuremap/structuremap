using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LiteralInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public interface ITarget
        {
        }

        public class ATarget : ITarget
        {
            public override string ToString()
            {
                return "the description of ATarget";
            }
        }

        [Test]
        public void Build_happy_path()
        {
            ATarget target = new ATarget();
            LiteralInstance instance = new LiteralInstance(target);
            Assert.AreSame(target, instance.Build(typeof (ITarget), new StubBuildSession()));
        }

        [Test]
        public void Can_be_part_of_PluginFamily()
        {
            ATarget target = new ATarget();
            LiteralInstance instance = new LiteralInstance(target);
            IDiagnosticInstance diagnosticInstance = instance;

            PluginFamily family1 = new PluginFamily(typeof (ATarget));
            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family1));

            PluginFamily family2 = new PluginFamily(GetType());
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family2));
        }

        [Test]
        public void Create_description_should_return_the_ToString_of_the_inner_instance()
        {
            LiteralInstance instance = new LiteralInstance(this);
            TestUtility.AssertDescriptionIs(instance, "Object:  " + ToString());
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void Throw_NullArgumentException_if_literal_instance_is_null()
        {
            LiteralInstance instance = new LiteralInstance(null);
        }
    }
}