using System;
using System.Reflection;

namespace StructureMap.AutoMocking
{
    public class MoqFactory
    {
        private readonly Type mockOpenType;

        public MoqFactory()
        {
            Assembly Moq = Assembly.Load("Moq");
            mockOpenType = Moq.GetType("Moq.Mock`1");
            if (mockOpenType == null)
                throw new InvalidOperationException("Unable to find Type Moq.Mock<T> in assembly " + Moq.Location);
        }

        public object CreateMock(Type type)
        {
            Type closedType = mockOpenType.MakeGenericType(new[] {type});
            PropertyInfo objectProperty = closedType.GetProperty("Object", type);
            object instance = Activator.CreateInstance(closedType);
            return objectProperty.GetValue(instance, null);
        }

        public object CreateMockThatCallsBase(Type type, object[] args)
        {
            Type closedType = mockOpenType.MakeGenericType(new[] {type});
            PropertyInfo callBaseProperty = closedType.GetProperty("CallBase", typeof (bool));
            PropertyInfo objectProperty = closedType.GetProperty("Object", type);
            ConstructorInfo constructor = closedType.GetConstructor(new[] {typeof (object[])});
            object instance = constructor.Invoke(new[] {args});
            callBaseProperty.SetValue(instance, true, null);
            return objectProperty.GetValue(instance, null);
        }
    }
}