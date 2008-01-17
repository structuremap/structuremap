using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Configuration.Mementos;
using StructureMap.Graph;

namespace StructureMap.Testing.Container
{
    [TestFixture]
    public class ExplicitArgumentTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            StructureMapConfiguration.ResetAll();
            StructureMapConfiguration.UseDefaultStructureMapConfigFile = false;
        }

        [TearDown]
        public void TearDown()
        {
            StructureMapConfiguration.ResetAll();
            ObjectFactory.Reset();

        }

        #endregion

        public void GetTypedArgumentsFromAnExplicitArgumentsMementoIfThereIsAnExplicitArgument()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                    Registry.Instance<ExplicitTarget>()
                        .UsingConcreteType<ExplicitTarget>()
                        .Child<IProvider>().IsConcreteType<RedProvider>()
                        .WithProperty("name").EqualTo("Jeremy")
                    );
            }

            InstanceMemento inner = pluginGraph.PluginFamilies[typeof (ExplicitTarget)].Source.GetAllMementos()[0];
            ExplicitArguments args = new ExplicitArguments();
            ExplicitArgumentMemento memento = new ExplicitArgumentMemento(args, inner);

            InstanceManager manager = new InstanceManager(pluginGraph);

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = manager.CreateInstance<ExplicitTarget>(memento);
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            args.Set<IProvider>(new BlueProvider());
            ExplicitTarget secondTarget = manager.CreateInstance<ExplicitTarget>(memento);
            Assert.IsInstanceOfType(typeof (BlueProvider), secondTarget.Provider);
        }


        public interface IExplicitTarget
        {
        }

        public class RedTarget : IExplicitTarget
        {
        }

        public class GreenTarget : IExplicitTarget
        {
        }

        public class ExplicitTarget : IExplicitTarget
        {
            private readonly string _name;
            private readonly IProvider _provider;

            public ExplicitTarget(string name, IProvider provider)
            {
                _name = name;
                _provider = provider;
            }


            public string Name
            {
                get { return _name; }
            }

            public IProvider Provider
            {
                get { return _provider; }
            }
        }

        public interface IProvider
        {
        }

        public class RedProvider : IProvider
        {
        }

        public class BlueProvider : IProvider
        {
        }

        [Test]
        public void NowDoItWithObjectFactoryItself()
        {
            StructureMapConfiguration.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                Registry.Instance<ExplicitTarget>()
                    .UsingConcreteType<ExplicitTarget>()
                    .Child<IProvider>().IsConcreteType<RedProvider>()
                    .WithProperty("name").EqualTo("Jeremy")
                );

            ObjectFactory.Reset();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            BlueProvider theBlueProvider = new BlueProvider();
            ExplicitTarget secondTarget = ObjectFactory.With<IProvider>(theBlueProvider).GetInstance<ExplicitTarget>();
            Assert.AreSame(theBlueProvider, secondTarget.Provider);
        }

        [Test]
        public void OverrideAPrimitiveWithObjectFactory()
        {
            StructureMapConfiguration.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                Registry.Instance<ExplicitTarget>()
                    .UsingConcreteType<ExplicitTarget>()
                    .Child<IProvider>().IsConcreteType<RedProvider>()
                    .WithProperty("name").EqualTo("Jeremy")
                );

            ObjectFactory.Reset();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = ObjectFactory.GetInstance<ExplicitTarget>();
            Assert.AreEqual("Jeremy", firstTarget.Name);

            // Now, set the explicit arg for IProvider
            ExplicitTarget secondTarget = ObjectFactory.With("name").EqualTo("Julia").GetInstance<ExplicitTarget>();
            Assert.AreEqual("Julia", secondTarget.Name);
        }

        [Test]
        public void OverridePrimitiveArgs()
        {
            PluginGraph pluginGraph = new PluginGraph();
            using (Registry registry = new Registry(pluginGraph))
            {
                registry.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                    Registry.Instance<ExplicitTarget>()
                        .UsingConcreteType<ExplicitTarget>()
                        .Child<IProvider>().IsConcreteType<RedProvider>()
                        .WithProperty("name").EqualTo("Jeremy")
                    );
            }

            InstanceMemento inner = pluginGraph.PluginFamilies[typeof (ExplicitTarget)].Source.GetAllMementos()[0];
            ExplicitArguments args = new ExplicitArguments();
            ExplicitArgumentMemento memento = new ExplicitArgumentMemento(args, inner);

            InstanceManager manager = new InstanceManager(pluginGraph);

            // Once without an explicit arg set
            Assert.AreEqual("Jeremy", manager.CreateInstance<ExplicitTarget>(memento).Name);

            // Now, set the explicit arg
            args.SetArg("name", "Max");
            Assert.AreEqual("Max", manager.CreateInstance<ExplicitTarget>(memento).Name);
        }

        [Test]
        public void PassExplicitArgsIntoInstanceManager()
        {
            Registry registry = new Registry();

            registry.ForRequestedType<ExplicitTarget>().TheDefaultIs(
                Registry.Instance<ExplicitTarget>()
                    .UsingConcreteType<ExplicitTarget>()
                    .Child<IProvider>().IsConcreteType<RedProvider>()
                    .WithProperty("name").EqualTo("Jeremy")
                );

            IInstanceManager manager = registry.BuildInstanceManager();

            ExplicitArguments args = new ExplicitArguments();

            // Get the ExplicitTarget without setting an explicit arg for IProvider
            ExplicitTarget firstTarget = manager.CreateInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (RedProvider), firstTarget.Provider);

            // Now, set the explicit arg for IProvider
            args.Set<IProvider>(new BlueProvider());
            ExplicitTarget secondTarget = manager.CreateInstance<ExplicitTarget>(args);
            Assert.IsInstanceOfType(typeof (BlueProvider), secondTarget.Provider);
        }

        [Test]
        public void RegisterAndFindServicesOnTheExplicitArgument()
        {
            ExplicitArguments args = new ExplicitArguments();
            Assert.IsNull(args.Get<IProvider>());

            RedProvider red = new RedProvider();
            args.Set<IProvider>(red);

            Assert.AreSame(red, args.Get<IProvider>());

            args.Set<IExplicitTarget>(new RedTarget());
            Assert.IsInstanceOfType(typeof (RedTarget), args.Get<IExplicitTarget>());
        }

        [Test]
        public void RegisterAndRetrieveArgs()
        {
            ExplicitArguments args = new ExplicitArguments();
            Assert.IsNull(args.GetArg("name"));

            args.SetArg("name", "Jeremy");
            Assert.AreEqual("Jeremy", args.GetArg("name"));

            args.SetArg("age", 34);
            Assert.AreEqual("34", args.GetArg("age"));
        }
    }
}