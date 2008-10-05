using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;
using StructureMap.Attributes;
using StructureMap.Emitting;
using StructureMap.Emitting.Parameters;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
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
                var plugin = new Plugin(typeof (ComplexRule));

                var _InstanceBuilderAssembly =
                    new InstanceBuilderAssembly(new[] {plugin});

                List<InstanceBuilder> list = _InstanceBuilderAssembly.Compile();
                builder = list[0];

                if (builder != null)
                {
                    rule = (ComplexRule) builder.BuildInstance(instance, new StubBuildSession());
                }
            }
            catch (Exception e)
            {
                ex = e;
                Debug.WriteLine(e.ToString());
            }
        }

        #endregion

        private InstanceBuilder builder;
        private Exception ex;
        private IConfiguredInstance instance;
        private ComplexRule rule;

        [Test]
        public void BoolProperty()
        {
            Assert.AreEqual(true, rule.Bool);
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
        public void can_get_the_parse_method_from_Enum()
        {
            Methods.ENUM_PARSE.ShouldNotBeNull();
        }

        [Test]
        public void DoubleProperty()
        {
            Assert.AreEqual(4, rule.Double);
        }

        [Test]
        public void EmitANoArgClass()
        {
            var plugin = new Plugin(typeof (NoArgClass));
            var _InstanceBuilderAssembly =
                new InstanceBuilderAssembly(new[] {plugin});
            List<InstanceBuilder> list = _InstanceBuilderAssembly.Compile();
            builder = list[0];

            object obj = builder.BuildInstance(new ConfiguredInstance(typeof (NoArgClass)), new StubBuildSession());
            obj.ShouldNotBeNull();
        }


        [Test]
        public void EmitAOneSetterClass()
        {
            var plugin = new Plugin(typeof (WithOneSetter));
            var _InstanceBuilderAssembly =
                new InstanceBuilderAssembly(new[] {plugin});
            List<InstanceBuilder> list = _InstanceBuilderAssembly.Compile();
            builder = list[0];

            object obj =
                builder.BuildInstance(
                    new ConfiguredInstance(typeof (WithOneSetter)).WithProperty("Name").EqualTo("Jeremy"),
                    new StubBuildSession());
            obj.ShouldNotBeNull();
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

    public class NoArgClass
    {
    }

    public class WithOneSetter
    {
        [SetterProperty]
        public string Name { get; set; }
    }
}