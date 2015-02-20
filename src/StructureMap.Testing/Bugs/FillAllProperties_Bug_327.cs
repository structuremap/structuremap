using NUnit.Framework;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class FillAllProperties_Bug_327
    {
        [Test]
        public void try_out_the_convention()
        {
            var container = new Container(_ =>
            {
                _.Policies.FillAllPropertiesOfType<ISettings>().Singleton().Use<Settings>();
            });

            // They'd be the same object because Settings is singleton scope
            var controller = container.GetInstance<HomeController>();
            controller.Settings.ShouldNotBeNull();
            controller.FromConstructor.ShouldNotBeNull();
        }

        public class HomeController
        {
            public ISettings Settings { get; set; }
            public HomeController(ISettings settings)
            {
                FromConstructor = settings;
            }

            // Has to be private to keep it from getting set
            public ISettings FromConstructor { get; private set; }
        }

        public interface ISettings
        {

        }

        public class Settings : ISettings
        {

        }
    }
}