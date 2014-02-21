using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Pipeline
{
    [TestFixture]
    public class LifecycleObjectCacheTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            cache = new LifecycleObjectCache();
        }

        #endregion

        private LifecycleObjectCache cache;

        [Test]
        public void get_for_uncached_instance_returns_passed_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);
            
            var cachedWidget = cache.Get(typeof(IWidget), instance, new StubBuildSession());

            Assert.AreEqual(aWidget, cachedWidget);
        }    
        
        [Test]
        public void get_for_uncached_instance_builds_instance()
        {
            var instance = new ObjectInstance(new AWidget());
            var mockBuildSession = MockRepository.GenerateMock<IBuildSession>();

            cache.Get(typeof(IWidget), instance, mockBuildSession);

            mockBuildSession.AssertWasCalled(session => session.BuildNewInOriginalContext(typeof(IWidget), instance));
        }      
        
        [Test]
        public void get_for_cached_instance_does_not_build_instance()
        {
            var instance = new ObjectInstance(new AWidget());
            var mockBuildSession = MockRepository.GenerateMock<IBuildSession>();
            cache.Get(typeof(IWidget), instance, new StubBuildSession());

            cache.Get(typeof(IWidget), instance, mockBuildSession);

            mockBuildSession.AssertWasNotCalled(session => session.BuildNewInOriginalContext(typeof(IWidget), instance));
        }       
        
        [Test]
        public void get_for_cached_instance_returns_cached_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);
            cache.Get(typeof(IWidget), instance, new StubBuildSession());
            
            var cachedWidget = cache.Get(typeof(IWidget), instance, new StubBuildSession());

            Assert.AreEqual(aWidget, cachedWidget);
        }     
        
        [Test]
        public void get_for_cached_instance_created_on_different_thread_returns_cached_instance()
        {
            var aWidget = new AWidget();
            var instance = new ObjectInstance(aWidget);
            cache.Get(typeof(IWidget), instance, new StubBuildSession());

            var threadStarted = new ManualResetEvent(false);
            Func<object> getDelegate =
                () =>
                {
                    threadStarted.Set();
                    return cache.Get(typeof (IWidget), instance, new StubBuildSession());
                };

            object cachedWidget = null;
            var asyncResult = getDelegate.BeginInvoke(ar => cachedWidget = getDelegate.EndInvoke(ar), null);

            // Wait forever for the thread pool thread to kick off
            threadStarted.WaitOne();
            // ...then wait 10ms for the Get call to complete
            asyncResult.AsyncWaitHandle.WaitOne(10);

            Assert.AreEqual(aWidget, cachedWidget);
        }   

        [Test]
        public void eject_a_disposable_object()
        {
            var disposable = MockRepository.GenerateMock<IDisposable>();
            var instance = new ObjectInstance(disposable);

            cache.Set(typeof (IWidget), instance, disposable);

            cache.Eject(typeof (IWidget), instance);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            disposable.AssertWasCalled(x => x.Dispose());
        }

        [Test]
        public void eject_a_non_disposable_object()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Set(typeof (IWidget), instance, widget);

            cache.Eject(typeof (IWidget), instance);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();
        }

        [Test]
        public void has()
        {
            var widget = new AWidget();
            var instance = new ObjectInstance(widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (Rule), instance, widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (IWidget), new ObjectInstance(new AWidget()), widget);

            cache.Has(typeof (IWidget), instance).ShouldBeFalse();

            cache.Set(typeof (IWidget), instance, widget);

            cache.Has(typeof (IWidget), instance).ShouldBeTrue();
        }


    }
}