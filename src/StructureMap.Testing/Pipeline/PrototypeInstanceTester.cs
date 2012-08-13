using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class PrototypeInstanceTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        public class PrototypeTarget : ICloneable, IEquatable<PrototypeTarget>
        {
            private string _name;


            public PrototypeTarget(string name)
            {
                _name = name;
            }

            public string Name { get { return _name; } set { _name = value; } }

            #region ICloneable Members

            public object Clone()
            {
                return MemberwiseClone();
            }

            #endregion

            #region IEquatable<PrototypeTarget> Members

            public bool Equals(PrototypeTarget prototypeTarget)
            {
                if (prototypeTarget == null) return false;
                return Equals(_name, prototypeTarget._name);
            }

            #endregion

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(this, obj)) return true;
                return Equals(obj as PrototypeTarget);
            }

            public override int GetHashCode()
            {
                return _name != null ? _name.GetHashCode() : 0;
            }
        }

        [Test]
        public void Build_a_clone()
        {
            var target = new PrototypeTarget("Jeremy");
            var instance = new PrototypeInstance(target);

            object returnedValue = instance.Build(typeof (PrototypeTarget), new StubBuildSession());

            Assert.AreEqual(target, returnedValue);
            Assert.AreNotSame(target, returnedValue);
        }

        [Test]
        public void Can_be_part_of_PluginFamily()
        {
            var target = new PrototypeTarget("Jeremy");
            var instance = new PrototypeInstance(target);
            IDiagnosticInstance diagnosticInstance = instance;

            var family1 = new PluginFamily(typeof (PrototypeTarget));
            Assert.IsTrue(diagnosticInstance.CanBePartOfPluginFamily(family1));

            var family2 = new PluginFamily(GetType());
            Assert.IsFalse(diagnosticInstance.CanBePartOfPluginFamily(family2));
        }

        [Test]
        public void Get_description()
        {
            var target = new PrototypeTarget("Jeremy");
            var instance = new PrototypeInstance(target);

            TestUtility.AssertDescriptionIs(instance, "Prototype of " + target);
        }
    }
}