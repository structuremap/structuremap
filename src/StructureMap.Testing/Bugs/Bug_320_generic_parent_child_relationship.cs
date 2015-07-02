using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using StructureMap.Graph;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug_320_generic_parent_child_relationship
    {
        public class Parent
        {
        };

        public class Child : Parent
        {
        }

        public class ConcreteChild : Child
        {
        }

        public interface IGeneric<in T> where T : class
        {
        }

        public class GenericClass1 : IGeneric<Parent>
        {
        }

        public class GenericClass2 : IGeneric<Child>
        {
        }

        //public class GenericClass3 : IGeneric<ConcreteChild> { }

        [Test]
        public void StructureMap_Resolves_Generic_Child_Classes()
        {
            typeof (IGeneric<ConcreteChild>).IsAssignableFrom(typeof (GenericClass1)).ShouldBeTrue();
            typeof (IGeneric<ConcreteChild>).IsAssignableFrom(typeof (GenericClass2)).ShouldBeTrue();
            //Assert.IsTrue(typeof(IGeneric<ConcreteChild>).IsAssignableFrom(typeof(GenericClass3)));

            var container = new Container(cfg =>
            {
                cfg.Scan(scan =>
                {
                    scan.TheCallingAssembly();
                    scan.ConnectImplementationsToTypesClosing(typeof (IGeneric<>));
                });
            });

            var what = container.WhatDoIHave();
            Debug.WriteLine(what);

            var instances = container.GetAllInstances<IGeneric<ConcreteChild>>();

            instances.Any(t => t.GetType() == typeof (GenericClass1)).ShouldBeTrue();
            instances.Any(t => t.GetType() == typeof (GenericClass2)).ShouldBeTrue();
        }
    }
}