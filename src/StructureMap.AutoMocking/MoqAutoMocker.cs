namespace StructureMap.AutoMocking
{
    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Moq
    /// </summary>
    /// <typeparam name="TARGETCLASS">The concrete class being tested</typeparam>
    public class MoqAutoMocker<TARGETCLASS> : AutoMocker<TARGETCLASS> where TARGETCLASS : class
    {
        public MoqAutoMocker()
        {
            _serviceLocator = new MoqServiceLocator();
            _container = new AutoMockedContainer(_serviceLocator);
        }
    }
}