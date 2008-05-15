using System;
using System.Drawing;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Graph;
using StructureMap.Testing.Widget2;
using StructureMap.Testing.Widget3;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class TypeRulesTester : TypeRules
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void IsString()
        {
            
            Assert.IsTrue(IsString(typeof(string)));
            Assert.IsFalse(IsString(typeof(int)));
        }

        [Test]
        public void IsPrimitive()
        {
            

            Assert.IsTrue(IsPrimitive(typeof(int)));
            Assert.IsTrue(IsPrimitive(typeof(bool)));
            Assert.IsTrue(IsPrimitive(typeof(double)));
            Assert.IsFalse(IsPrimitive(typeof(string)));
            Assert.IsFalse(IsPrimitive(typeof(BreedEnum)));
            Assert.IsFalse(IsPrimitive(typeof(IGateway)));
        }

        [Test]
        public void IsSimple()
        {
            

            Assert.IsTrue(IsSimple(typeof(int)));
            Assert.IsTrue(IsSimple(typeof(bool)));
            Assert.IsTrue(IsSimple(typeof(double)));
            Assert.IsTrue(IsSimple(typeof(string)));
            Assert.IsTrue(IsSimple(typeof(BreedEnum)));
            Assert.IsFalse(IsSimple(typeof(IGateway)));
        }

        [Test]
        public void IsEnum()
        {
            

            Assert.IsFalse(IsEnum(typeof(int)));
            Assert.IsFalse(IsEnum(typeof(bool)));
            Assert.IsFalse(IsEnum(typeof(double)));
            Assert.IsFalse(IsEnum(typeof(string)));
            Assert.IsTrue(IsEnum(typeof(BreedEnum)));
            Assert.IsFalse(IsEnum(typeof(IGateway)));
        }

        [Test]
        public void IsChild()
        {
            

            Assert.IsFalse(IsChild(typeof(int)));
            Assert.IsFalse(IsChild(typeof(bool)));
            Assert.IsFalse(IsChild(typeof(double)));
            Assert.IsFalse(IsChild(typeof(string)));
            Assert.IsFalse(IsChild(typeof(BreedEnum)));
            Assert.IsFalse(IsChild(typeof(IGateway[])));
            Assert.IsTrue(IsChild(typeof(IGateway)));
        }

        [Test]
        public void IsChildArray()
        {
            

            Assert.IsFalse(IsChildArray(typeof(int)));
            Assert.IsFalse(IsChildArray(typeof(bool)));
            Assert.IsFalse(IsChildArray(typeof(double)));
            Assert.IsFalse(IsChildArray(typeof(double[])));
            Assert.IsFalse(IsChildArray(typeof(string)));
            Assert.IsFalse(IsChildArray(typeof(string[])));
            Assert.IsFalse(IsChildArray(typeof(BreedEnum)));
            Assert.IsTrue(IsChildArray(typeof(IGateway[])));
            Assert.IsFalse(IsChildArray(typeof(IGateway)));
        }
    }
}
