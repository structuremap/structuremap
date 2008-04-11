using System;
using NUnit.Framework;
using StructureMap.Caching;

namespace StructureMap.Testing.Caching
{
    [TestFixture]
    public class StorageAndCacheItemTester
    {
        [Test]
        public void StoreAndRetrieveFromCloneStorageStrategy()
        {
            CloneStorageStrategy strategy = new CloneStorageStrategy();

            ICacheItem item = strategy.BuildCacheItem("Red");
            Assert.IsTrue(item is CloneCacheItem);

            CacheTarget target1 = new CacheTarget("Red");
            item.Value = target1;

            CacheTarget target2 = (CacheTarget) item.Value;

            Assert.AreEqual(target1, target2);
            Assert.IsFalse(ReferenceEquals(target1, target2));
        }


        [Test]
        public void StoreAndRetrieveFromSerializationStorageStrategy()
        {
            SerializationStorageStrategy strategy = new SerializationStorageStrategy();

            ICacheItem item = strategy.BuildCacheItem("Red");
            Assert.IsTrue(item is SerializationCacheItem);

            CacheTarget target1 = new CacheTarget("Red");
            item.Value = target1;

            CacheTarget target2 = (CacheTarget) item.Value;

            Assert.AreEqual(target1, target2);
            Assert.IsFalse(ReferenceEquals(target1, target2));
        }


        [Test]
        public void StoreAndRetrieveFromSharedStorageStrategy()
        {
            SharedStorageStrategy strategy = new SharedStorageStrategy();

            ICacheItem item = strategy.BuildCacheItem("Red");
            Assert.IsTrue(item is SharedCacheItem);

            CacheTarget target1 = new CacheTarget("Red");
            item.Value = target1;

            CacheTarget target2 = (CacheTarget) item.Value;

            Assert.AreEqual(target1, target2);
            Assert.AreSame(target1, target2);
        }

        [Test,
         ExpectedException(typeof (ApplicationException),
            ExpectedMessage = "Type StructureMap.Testing.Caching.CacheTarget2 is does not implement the ICloneable interface")]
        public void ThrowsExceptionWhenNotCloneable()
        {
            CloneCacheItem item = new CloneCacheItem("key");
            item.Value = new CacheTarget2();
        }

        [Test,
         ExpectedException(typeof (ApplicationException),
            ExpectedMessage = "Exception when trying to serialize type StructureMap.Testing.Caching.CacheTarget2")]
        public void ThrowsExceptionWhenNotSerializable()
        {
            SerializationCacheItem item = new SerializationCacheItem("key");
            item.Value = new CacheTarget2();
        }
    }


    [Serializable]
    public class CacheTarget : ICloneable
    {
        private string _color;

        public CacheTarget(string color)
        {
            _color = color;
        }

        public string Color
        {
            get { return _color; }
        }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        public override bool Equals(object obj)
        {
            return _color.Equals(((CacheTarget) obj).Color);
        }

        public override int GetHashCode()
        {
            return _color.GetHashCode();
        }
    }

    /// <summary>
    /// The point of this class is that it is not ICloneable or Serializable
    /// </summary>
    public class CacheTarget2
    {
    }
}