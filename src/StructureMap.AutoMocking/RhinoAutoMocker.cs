using System;

namespace StructureMap.AutoMocking
{
    public delegate void GenericVoidMethod<T>(T target);

    public delegate void VoidMethod();

    public enum MockMode
    {
        RecordAndReplay,
        AAA
    }

    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Rhino.Mocks
    /// </summary>
    /// <typeparam name="T">The concrete class being tested</typeparam>
    public class RhinoAutoMocker<T> : AutoMocker<T> where T : class
    {
        public RhinoAutoMocker()
            : this(MockMode.AAA)
        {
        }

        public RhinoAutoMocker(MockMode mockMode)
        {
            ServiceLocator = createLocator(mockMode);
            Container = new AutoMockedContainer(ServiceLocator);
        }

        private ServiceLocator createLocator(MockMode mode)
        {
            switch (mode)
            {
                case MockMode.RecordAndReplay:
                    return new RhinoMocksClassicServiceLocator();
                case MockMode.AAA:
                    return new RhinoMocksAAAServiceLocator();
                default:
                    throw new InvalidOperationException("Unsupported MockMode " + mode);
            }
        }
    }
}