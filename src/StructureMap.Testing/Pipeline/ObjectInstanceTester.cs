using System;
using NUnit.Framework;
using StructureMap.Building;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class ObjectInstanceTester
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
        public void to_dependency_source()
        {
            var gateway = new StubbedGateway();
            new ObjectInstance(gateway).ToDependencySource(typeof (IGateway))
                .ShouldEqual(Constant.For<IGateway>(gateway));
        }

        [Test]
        public void Build_happy_path()
        {
            var target = new ATarget();
            var instance = new ObjectInstance(target);

            instance.Build<ATarget>().ShouldBeTheSameAs(target);
        }

        [Test]
        public void Create_description_should_return_the_ToString_of_the_inner_instance()
        {
            new ObjectInstance(this)
                .Description.ShouldEqual("Object:  " + ToString());
        }

        [Test]
        public void Throw_NullArgumentException_if_literal_instance_is_null()
        {
            Exception<ArgumentNullException>.ShouldBeThrownBy(() => {
                new ObjectInstance(null);
            });
        }
    }
}