using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using StructureMap.Util;

namespace StructureMap.Testing.Util
{
    [TestFixture]
    public class TypeDictionaryTester
    {
        private TypeDictionary<string> _dictionary;

        #region Types used for testing

        class NonGeneric { }
        class Generic<T> { }

        #endregion

        [SetUp]
        public void SetUp()
        {
            _dictionary = new TypeDictionary<string>();
        }

        [Test]
        public void It_can_get_NonGeneric_key()
        {
            _dictionary.Add(typeof(NonGeneric), "yup");
            string val = null;
            Assert.That(_dictionary.TryGetValue(typeof(NonGeneric), out val));
        }

        [Test]
        public void It_can_get_Generic_key_directly_as_open_generic()
        {
            _dictionary.Add(typeof(Generic<>), "yup");
            string val = null;
            Assert.That(_dictionary.TryGetValue(typeof(Generic<>), out val));
        }

        [Test]
        public void It_can_get_Generic_key_indirectly_as_closed_generic()
        {
            _dictionary.Add(typeof(Generic<>), "yup");
            string val = null;
            Assert.That(_dictionary.TryGetValue(typeof(Generic<string>), out val));
        }
    }
}
