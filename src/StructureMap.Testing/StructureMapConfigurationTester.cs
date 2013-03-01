using System;
using System.Collections.Generic;
using System.Xml;
using NUnit.Framework;
using StructureMap.Configuration;
using StructureMap.Configuration.DSL;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing
{
    [TestFixture]
    public class StructureMapConfigurationTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            DataMother.RestoreStructureMapConfig();
        }

        #endregion

        
        public class WebRegistry : Registry
        {
        }

        public class CoreRegistry : Registry
        {
        }

        [Test]
        public void StructureMap_functions_without_StructureMapconfig_file_in_the_default_mode()
        {
            DataMother.RemoveStructureMapConfig();

            ObjectFactory.Initialize(x => { });
        }

        [Test(
            Description =
                "Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en"
            )]
        public void TheDefaultInstance_has_a_dependency_upon_a_Guid_NewGuid_lambda_generated_instance()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<Guid>().Use(() => Guid.NewGuid());
                x.For<IFoo>().Use<Foo>();
            });


            Assert.That(ObjectFactory.GetInstance<IFoo>().SomeGuid != Guid.Empty);
        }

        [Test(
            Description =
                "Guid test based on problems encountered by Paul Segaro. See http://groups.google.com/group/structuremap-users/browse_thread/thread/34ddaf549ebb14f7?hl=en"
            )]
        public void TheDefaultInstanceIsALambdaForGuidNewGuid()
        {
            ObjectFactory.Initialize(x =>
            {
                x.For<Guid>().Use(() => Guid.NewGuid());
            });


            Assert.That(ObjectFactory.GetInstance<Guid>() != Guid.Empty);
        }
    }

    public interface IFoo
    {
        Guid SomeGuid { get; set; }
    }

    public class Foo : IFoo
    {
        public Foo(Guid someGuid)
        {
            SomeGuid = someGuid;
        }

        #region IFoo Members

        public Guid SomeGuid { get; set; }

        #endregion
    }

    public interface ISomething
    {
    }

    public class Something : ISomething
    {
        public Something()
        {
            throw new ApplicationException("You can't make me!");
        }
    }
}