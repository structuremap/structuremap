using System;
using NUnit.Framework;
using StructureMap.Caching;
using StructureMap.Caching.Expirations;

namespace StructureMap.Testing.Caching
{
    [TestFixture]
    public class ExpirationTester
    {
        [Test]
        public void ExpireOnCreatedAgeOfCacheItem()
        {
            SharedCacheItem item = new SharedCacheItem("key");

            AbsoluteTimeExpirationPolicy policy = new AbsoluteTimeExpirationPolicy(5);

            policy.Calculate(item.Created);
            Assert.IsFalse(policy.HasExpired(item));

            policy.Calculate(item.Created.AddMinutes(6));
            Assert.IsTrue(policy.HasExpired(item));
        }

        [Test]
        public void ExpireOnLastAccessedAgeOfCacheItem()
        {
            MockCacheItem item = new MockCacheItem();

            DateTime createdTime = new DateTime(2004, 7, 1);

            item.Created = createdTime;

            SlidingTimeExpirationPolicy policy = new SlidingTimeExpirationPolicy(5);
            policy.Calculate(createdTime.AddMinutes(6));

            // Not expired according to the LastAccessed time
            item.LastAccessed = createdTime.AddMinutes(10);
            Assert.IsFalse(policy.HasExpired(item));


            policy.Calculate(createdTime.AddMinutes(16));
            Assert.IsTrue(policy.HasExpired(item));
        }
    }

    /// <summary>
    /// Static mock class to facilitate testing with ICacheItem's.  Using a static mock because of 
    /// past difficulties using NMock with value types
    /// </summary>
    public class MockCacheItem : ICacheItem
    {
        private int _accesses;
        private DateTime _created;
        private object _key;
        private DateTime _lastAccessed;
        private object _value;

        #region ICacheItem Members

        public DateTime Created
        {
            get { return _created; }
            set { _created = value; }
        }

        public DateTime LastAccessed
        {
            get { return _lastAccessed; }
            set { _lastAccessed = value; }
        }

        public object Key
        {
            get { return _key; }
            set { _key = value; }
        }

        public int Accesses
        {
            get { return _accesses; }
            set { _accesses = value; }
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }

        #endregion
    }
}