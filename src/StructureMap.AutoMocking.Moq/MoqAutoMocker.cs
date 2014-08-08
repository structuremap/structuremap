namespace StructureMap.AutoMocking.Moq
{
    /// <summary>
    /// Provides an "Auto Mocking Container" for the concrete class TARGETCLASS using Moq
    /// </summary>
    /// <typeparam name="T">The concrete class being tested</typeparam>
    public class MoqAutoMocker<T> : AutoMocker<T> where T : class
    {
        public MoqAutoMocker() : base(new MoqServiceLocator())
        {
            ServiceLocator = new MoqServiceLocator();
        }
    }
}