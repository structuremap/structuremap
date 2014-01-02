using NUnit.Framework;
using Rhino.Mocks;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Graph.Interceptors
{
    [TestFixture]
    public class InterceptorLibraryTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _interceptor1 = new MockTypeInterceptor(typeof (string));
            _interceptor2 = new MockTypeInterceptor(typeof (int), typeof (double));
            _interceptor3 = new MockTypeInterceptor(typeof (string), typeof (bool));
            _interceptor4 = new MockTypeInterceptor(typeof (string), typeof (double));

            _library = new InterceptorLibrary();
            _library.AddInterceptor(_interceptor1);
            _library.AddInterceptor(_interceptor2);
            _library.AddInterceptor(_interceptor3);
            _library.AddInterceptor(_interceptor4);
        }

        #endregion

        private MockTypeInterceptor _interceptor1;
        private MockTypeInterceptor _interceptor2;
        private MockTypeInterceptor _interceptor3;
        private MockTypeInterceptor _interceptor4;
        private InterceptorLibrary _library;

        [Test]
        public void Find_All_Of_The_Interceptors_For_A_Type_On_Multiple_Passes()
        {
            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptors(typeof (string)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptors(typeof (double)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2}, _library.FindInterceptors(typeof (int)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor3}, _library.FindInterceptors(typeof (bool)));

            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptors(typeof (string)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptors(typeof (double)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2}, _library.FindInterceptors(typeof (int)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor3}, _library.FindInterceptors(typeof (bool)));

            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptors(typeof (string)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptors(typeof (double)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2}, _library.FindInterceptors(typeof (int)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor3}, _library.FindInterceptors(typeof (bool)));

            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptors(typeof (string)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptors(typeof (double)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2}, _library.FindInterceptors(typeof (int)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor3}, _library.FindInterceptors(typeof (bool)));
        }

        [Test]
        public void Find_All_Of_The_Interceptors_For_A_Type_On_The_First_Pass()
        {
            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptors(typeof (string)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptors(typeof (double)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2}, _library.FindInterceptors(typeof (int)));
            Assert.AreEqual(new TypeInterceptor[] {_interceptor3}, _library.FindInterceptors(typeof (bool)));
        }

        [Test]
        public void Find_CompoundInterceptor_For_A_Type_On_The_First_Pass()
        {
            Assert.AreEqual(new TypeInterceptor[] {_interceptor1, _interceptor3, _interceptor4},
                _library.FindInterceptor(typeof (string)).Interceptors);
            Assert.AreEqual(new TypeInterceptor[] {_interceptor2, _interceptor4},
                _library.FindInterceptor(typeof (double)).Interceptors);
        }

        [Test]
        public void Import_from_gets_all_interceptors_and_resets_the_type_filter()
        {
            var sourceLibrary = new InterceptorLibrary();
            sourceLibrary.AddInterceptor(new MockTypeInterceptor(typeof (string)));
            sourceLibrary.FindInterceptor(typeof (string));

            var destinationLibrary = new InterceptorLibrary();
            destinationLibrary.AddInterceptor(new MockTypeInterceptor(typeof (string)));

            destinationLibrary.ImportFrom(sourceLibrary);

            var interceptors = destinationLibrary.FindInterceptors(typeof (string));
            Assert.AreEqual(2, interceptors.Length);
        }


        [Test]
        public void
            When_Interceptors_Are_Requested_For_A_Type_For_The_First_Time_The_Library_Will_Scan_All_The_TypeInterceptors
            ()
        {
            var mocks = new MockRepository();
            var interceptor1 = mocks.StrictMock<TypeInterceptor>();
            var interceptor2 = mocks.StrictMock<TypeInterceptor>();
            var interceptor3 = mocks.StrictMock<TypeInterceptor>();

            _library.AddInterceptor(interceptor1);
            _library.AddInterceptor(interceptor2);
            _library.AddInterceptor(interceptor3);

            var type = typeof (string);
            Expect.Call(interceptor1.MatchesType(type)).Return(true);
            Expect.Call(interceptor2.MatchesType(type)).Return(true);
            Expect.Call(interceptor3.MatchesType(type)).Return(true);

            mocks.ReplayAll();
            _library.FindInterceptors(type);
            mocks.VerifyAll();
        }

        [Test]
        public void When_Interceptors_Are_Requested_For_The_Second_Time_The_Library_Will_NOT_Scan_The_Interceptors_Again
            ()
        {
            var mocks = new MockRepository();
            var interceptor1 = mocks.StrictMock<TypeInterceptor>();
            var interceptor2 = mocks.StrictMock<TypeInterceptor>();
            var interceptor3 = mocks.StrictMock<TypeInterceptor>();

            _library.AddInterceptor(interceptor1);
            _library.AddInterceptor(interceptor2);
            _library.AddInterceptor(interceptor3);

            var type = typeof (string);
            Expect.Call(interceptor1.MatchesType(type)).Return(true);
            Expect.Call(interceptor2.MatchesType(type)).Return(true);
            Expect.Call(interceptor3.MatchesType(type)).Return(true);

            mocks.ReplayAll();
            _library.FindInterceptors(type);
            _library.FindInterceptors(type);
            _library.FindInterceptors(type);
            _library.FindInterceptors(type);
            _library.FindInterceptors(type);
            mocks.VerifyAll();
        }
    }
}