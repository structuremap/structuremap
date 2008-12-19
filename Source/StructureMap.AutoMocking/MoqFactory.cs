using System;
using System.Reflection;

namespace StructureMap.AutoMocking
{
    public class MoqFactory
    {
        private readonly Type mockOpenType;

        public MoqFactory()
        {
            var Moq = Assembly.Load("Moq");
            mockOpenType = Moq.GetType("Moq.Mock`1");
            if (mockOpenType == null) throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + Moq.Location);
        }

        public object CreateMock(Type type)
        {
            var closedType = mockOpenType.MakeGenericType(new[] {type});
            var objectProperty = closedType.GetProperty("Object", type);
            var instance = Activator.CreateInstance(closedType);
            return objectProperty.GetValue(instance, null);
        }

        public object CreateMockThatCallsBase(Type type, object[] args)
        {
            var closedType = mockOpenType.MakeGenericType(new[] { type });
            var callBaseProperty = closedType.GetProperty("CallBase", typeof(bool));
            var objectProperty = closedType.GetProperty("Object", type);
            var constructor = closedType.GetConstructor(new[]{typeof(object[])});
            var instance = constructor.Invoke(new[]{args});
            callBaseProperty.SetValue(instance, true, null);
            return objectProperty.GetValue(instance, null);
        }
    }
}