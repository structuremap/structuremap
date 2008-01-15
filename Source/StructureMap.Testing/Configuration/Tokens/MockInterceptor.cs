using System;
using StructureMap.Configuration.Mementos;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Configuration.Tokens
{
    [Pluggable("Mock")]
    public class MockInterceptor : InstanceFactoryInterceptor
    {
        public MockInterceptor(bool success) : base()
        {
            if (!success)
            {
                throw new ApplicationException("Bad");
            }
        }

        public static InstanceMemento CreateFailureMemento()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Mock", "failure");
            memento.SetProperty("success", false.ToString());

            return memento;
        }

        public static InstanceMemento CreateSuccessMemento()
        {
            MemoryInstanceMemento memento = new MemoryInstanceMemento("Mock", "failure");
            memento.SetProperty("success", true.ToString());

            return memento;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}