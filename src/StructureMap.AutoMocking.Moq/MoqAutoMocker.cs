namespace StructureMap.AutoMocking.Moq
{
    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Moq
    /// </summary>
    /// <typeparam name="T">The concrete class being tested</typeparam>
    public class MoqAutoMocker<T> : AutoMocker<T> where T : class
    {
        public MoqAutoMocker()
        {
            _serviceLocator = new MoqServiceLocator();
            _container = new AutoMockedContainer(_serviceLocator);
        }
    }
}