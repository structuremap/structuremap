using System;
using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using StructureMap.Emitting;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class EmittingTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            instance = ComplexRule.GetInstance();

            try
            {
                Plugin plugin = Plugin.CreateImplicitPlugin(typeof (ComplexRule));

                InstanceBuilderAssembly _InstanceBuilderAssembly =
                    new InstanceBuilderAssembly("StructureMap.EmittingTesterAssembly", typeof (Rule));

                _InstanceBuilderAssembly.AddPlugin(plugin);
                assem = _InstanceBuilderAssembly.Compile();

                if (assem != null)
                {
                    string builderName = plugin.GetInstanceBuilderClassName();

                    builder = assem.CreateInstance(builderName) as InstanceBuilder;
                }

                if (builder != null)
                {
                    rule = (ComplexRule) builder.BuildInstance(instance, new InstanceManager());
                }
            }
            catch (Exception e)
            {
                ex = e;
                Debug.WriteLine(e.ToString());
            }
        }

        #endregion

        private Assembly assem;
        private InstanceBuilder builder;
        private Exception ex;
        private IConfiguredInstance instance;
        private ComplexRule rule;

        public EmittingTester()
        {
        }

        [Test]
        public void BoolProperty()
        {
            Assert.AreEqual(true, rule.Bool);
        }


        [Test]
        public void BuiltTheAssembly()
        {
            Assert.IsNotNull(assem);
        }

        [Test]
        public void BuiltTheInstanceBuilder()
        {
            Assert.IsNotNull(builder);
        }

        [Test]
        public void ByteProperty()
        {
            Assert.AreEqual(3, rule.Byte);
        }

        [Test]
        public void ConcreteTypeKey()
        {
            Assert.AreEqual("Complex", builder.ConcreteTypeKey);
        }

        [Test]
        public void DoubleProperty()
        {
            Assert.AreEqual(4, rule.Double);
        }

        [Test]
        public void GotRule()
        {
            Assert.IsNotNull(rule);
        }

        [Test]
        public void IntProperty()
        {
            Assert.AreEqual(1, rule.Int);
        }

        [Test]
        public void LongProperty()
        {
            Assert.AreEqual(2, rule.Long);
        }

        [Test]
        public void NoException()
        {
            if (ex != null)
            {
                Console.Out.WriteLine(ex.Message);
                Console.Out.WriteLine(ex.Source);
                Console.Out.WriteLine(ex.StackTrace);

                Assert.Fail("Had an Exception!!!");
            }
        }


        [Test]
        public void String2Property()
        {
            Assert.AreEqual("Green", rule.String2);
        }

        [Test]
        public void StringProperty()
        {
            Assert.AreEqual("Red", rule.String);
        }
    }
}