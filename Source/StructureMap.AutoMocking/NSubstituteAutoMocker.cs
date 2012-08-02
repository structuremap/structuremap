namespace StructureMap.AutoMocking
{
    public class NSubstituteAutoMocker<T> : AutoMocker<T> where T : class
    {
        public NSubstituteAutoMocker()
        {
            _serviceLocator = new NSubstituteServiceLocator();
            _container = new AutoMockedContainer(_serviceLocator);
        }
    }
}
