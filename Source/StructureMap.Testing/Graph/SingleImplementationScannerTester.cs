using System;
using System.Collections.Generic;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Util;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class SingleImplementationScannerTester 
    {
        [Test]
        public void registers_plugins_that_only_have_a_single_implementation()
        {
            var container = new Container(registry =>
            {
                registry.Scan(x =>
                {
                    x.TheCallingAssembly();
                    x.IncludeNamespaceContainingType<SingleImplementationScannerTester>();
                    x.With(new SingleImplementationScanner());
                });
            });

            container.GetInstance<IOnlyHaveASingleConcreteImplementation>()
                .ShouldBeOfType<MyNameIsNotConventionallyRelatedToMyInterface>();
        }

        [Test]
        public void other()
        {
            typeof(Type).IsValueType.ShouldBeFalse();
        }
    }


    public interface IOnlyHaveASingleConcreteImplementation
    {
        
    }
    public class MyNameIsNotConventionallyRelatedToMyInterface : IOnlyHaveASingleConcreteImplementation
    {
        
    }
}