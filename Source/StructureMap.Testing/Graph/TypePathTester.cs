using System;
using NUnit.Framework;
using StructureMap.Graph;
using StructureMap.Testing.GenericWidgets;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Graph
{
    [TestFixture]
    public class TypePathTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void can_parse_assembly_qualified_generics()
        {
            Type sampleGenericType = typeof (IConcept<AWidget>);
            string genericAssemblyQualifiedName = sampleGenericType.AssemblyQualifiedName;

            var path = new TypePath(genericAssemblyQualifiedName);
            path.FindType().ShouldEqual(sampleGenericType);
        }

        [Test]
        public void CanBuildTypeCreatedFromType()
        {
            var path = new TypePath(GetType());
            path.FindType();
        }
    }
}