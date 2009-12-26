using System;
using System.Reflection;

namespace StructureMap.AutoMocking
{
    public class RhinoMockRepositoryProxy
    {
        private readonly Func<Type, object[], object> _DynamicMock;
        private readonly object _instance;
        private readonly Func<Type, object[], object> _PartialMock;
        private readonly Action<object> _Replay;

        public RhinoMockRepositoryProxy()
        {
            // We may consider allowing the consumer to pass in the MockRepository Type so we can avoid any possible Assembly conflict issues.
            // (The assumption being that their test project already has a reference to Rhino.Mocks.)
            // ex: var myclass = RhinoAutoMocker<MyClass>(MockMode.AAA, typeof(MockRepository)
            Assembly RhinoMocks = Assembly.Load("Rhino.Mocks");
            Type mockRepositoryType = RhinoMocks.GetType("Rhino.Mocks.MockRepository");
            if (mockRepositoryType == null)
                throw new InvalidOperationException("Unable to find Type Rhino.Mocks.MockRepository in assembly " +
                                                    RhinoMocks.Location);

            _instance = Activator.CreateInstance(mockRepositoryType);
            MethodInfo methodInfo = mockRepositoryType.GetMethod("DynamicMock", new[] {typeof (Type), typeof (object[])});
            if (methodInfo == null)
                throw new InvalidOperationException(
                    "Unable to find method DynamicMock(Type, object[]) on MockRepository.");
            _DynamicMock =
                (Func<Type, object[], object>)
                Delegate.CreateDelegate(typeof (Func<Type, object[], object>), _instance, methodInfo);

            methodInfo = mockRepositoryType.GetMethod("PartialMock", new[] {typeof (Type), typeof (object[])});
            if (methodInfo == null)
                throw new InvalidOperationException(
                    "Unable to find method PartialMock(Type, object[] on MockRepository.");
            _PartialMock =
                (Func<Type, object[], object>)
                Delegate.CreateDelegate(typeof (Func<Type, object[], object>), _instance, methodInfo);


            methodInfo = mockRepositoryType.GetMethod("Replay", new[] {typeof (object)});
            if (methodInfo == null)
                throw new InvalidOperationException("Unable to find method Replay(object) on MockRepository.");
            _Replay = (Action<object>) Delegate.CreateDelegate(typeof (Action<object>), _instance, methodInfo);
        }

        public object DynamicMock(Type type)
        {
            return _DynamicMock(type, null);
        }

        public object PartialMock(Type type, object[] args)
        {
            return _PartialMock(type, args);
        }

        public void Replay(object mock)
        {
            _Replay(mock);
        }
    }
}