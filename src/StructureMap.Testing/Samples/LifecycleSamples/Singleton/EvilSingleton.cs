namespace StructureMap.Testing.Samples.LifecycleSamples.Singleton
{
    // SAMPLE: evil-singleton
    public class EvilSingleton
    {
        public static readonly EvilSingleton Instance = new EvilSingleton();

        private EvilSingleton()
        {
        }

        public void DoSomething()
        {
            // do something with the static data here
        }
    }


    public class EvilSingletonUser
    {
        public void DoWork()
        {
            EvilSingleton.Instance.DoSomething();
        }
    }

    // ENDSAMPLE
}