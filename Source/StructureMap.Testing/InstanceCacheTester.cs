using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Pipeline;

namespace StructureMap.Testing
{
    [TestFixture]
    public class InstanceCacheTester
    {
        [Test]
        public void call_on_each_test()
        {
            var target1 = MockRepository.GenerateMock<ITypeTarget>();
            var target2 = MockRepository.GenerateMock<ITypeTarget>();
            var target3 = MockRepository.GenerateMock<ITypeTarget>();
            var target4 = MockRepository.GenerateMock<ITypeTarget>();

            var cache = new InstanceCache();
            cache.Set(typeof(int), new SmartInstance<int>(), target1);
            cache.Set(typeof(int), new SmartInstance<int>(), new object());
            cache.Set(typeof(int), new SmartInstance<int>(), new object());
            cache.Set(typeof(bool), new SmartInstance<int>(), target2);
            cache.Set(typeof(bool), new SmartInstance<int>(), new object());
            cache.Set(typeof(string), new SmartInstance<int>(), target3);
            cache.Set(typeof(string), new SmartInstance<int>(), new object());
            cache.Set(typeof(string), new SmartInstance<int>(), new object());
            cache.Set(typeof(string), new SmartInstance<int>(), target4);

            cache.Each<ITypeTarget>(x => x.Go());

            target1.AssertWasCalled(x => x.Go());
            target2.AssertWasCalled(x => x.Go());
            target3.AssertWasCalled(x => x.Go());
            target4.AssertWasCalled(x => x.Go());
        }
    }

    public interface ITypeTarget
    {
        void Go();
    }
}