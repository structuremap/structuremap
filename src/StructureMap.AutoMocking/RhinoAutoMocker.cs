using System;

namespace StructureMap.AutoMocking
{
    public delegate void GenericVoidMethod<TARGETCLASS>(TARGETCLASS target);

    public delegate void VoidMethod();

    public enum MockMode
    {
        RecordAndReplay,
        AAA
    }

    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Rhino.Mocks
    /// </summary>
    /// <typeparam name="TARGETCLASS">The concrete class being tested</typeparam>
    public class RhinoAutoMocker<TARGETCLASS> : AutoMocker<TARGETCLASS> where TARGETCLASS : class
    {
        public RhinoAutoMocker()
            : this(MockMode.AAA)
        {
        }

        public RhinoAutoMocker(MockMode mockMode)
        {
            _serviceLocator = createLocator(mockMode);
            _container = new AutoMockedContainer(_serviceLocator);
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