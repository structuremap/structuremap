using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;
using StructureMap.Util;

namespace StructureMap.Testing.Util
{
    [TestFixture]
    public class Cache_multi_threading_basher
    {
        private readonly IList<Guid> guids = new List<Guid>();
        private Cache<Guid, Guid> theCache;
        private GuidRequester[] requesters;

        [SetUp]
        public void SetUp()
        {
            for (var i = 0; i < 10; i++)
            {
                guids.Add(Guid.NewGuid());
            }

            theCache = new Cache<Guid, Guid>(key => Guid.NewGuid());

            requesters = new[]
            {
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester(),
                new GuidRequester()
            };
        }

        [Test]
        public void start_a_task_for_each_requester_that_would_hit_the_same_cache()
        {
            var tasks = requesters.Select(x => Task.Factory.StartNew(() => { x.Start(theCache, guids); })).ToArray();

            Task.WaitAll(tasks);

            var byKey = requesters.SelectMany(x => x.Holders).GroupBy(x => x.Key);
            byKey.Each(group => { group.GroupBy(x => x.Value).Count().ShouldBe(1); });
        }
    }

    public class GuidRequester
    {
        public readonly IList<GuidHolder> Holders = new List<GuidHolder>();

        public void Start(Cache<Guid, Guid> cache, IList<Guid> list)
        {
            for (var i = 0; i < 10; i++)
            {
                foreach (var key in list)
                {
                    Holders.Add(new GuidHolder
                    {
                        Key = key,
                        Value = cache[key]
                    });
                }
            }
        }
    }

    public class GuidHolder
    {
        public Guid Key;
        public Guid Value;
    }
}