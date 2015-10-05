using StructureMap.Pipeline;

namespace StructureMap.Testing.Examples
{
    public class UsingIConfiguredInstance
    {
        public class ImportantService { }


        public void Modify()
        {
            IConfiguredInstance instance = 
                new ConfiguredInstance(typeof(ImportantService));


            instance.SetLifecycleTo(new SingletonLifecycle());

            instance.SetLifecycleTo<SingletonLifecycle>();

            instance.Singleton();


        } 
    }
}