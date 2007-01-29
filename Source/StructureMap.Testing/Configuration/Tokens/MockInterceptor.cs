using System;
using StructureMap.Interceptors;

namespace StructureMap.Testing.Configuration.Tokens
{
    [Pluggable("Mock")]
    public class MockInterceptor : InstanceFactoryInterceptor
    {
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

        public MockInterceptor(bool success) : base()
        {
            if (!success)
            {
                throw new ApplicationException("Bad");
            }
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }
    }
}