using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;
using StructureMap.Testing.Widget2;

namespace StructureMap.Testing.Configuration.DSL
{
    [TestFixture]
    public class profiles_acceptance_tester : Registry
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
        }

        #endregion

        [Test]
        public void Add_default_instance_by_lambda()
        {
            string theProfileName = "something";

            IContainer container = new Container(r =>
            {
                r.Profile(theProfileName, x =>
                {
                    x.For<IWidget>().Use(() => new AWidget());
                    x.For<Rule>().Use(() => new DefaultRule());
                });




            });

            container.SetDefaultsToProfile(theProfileName);


            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<DefaultRule>();
        }


        [Test]
        public void Add_default_instance_by_lambda2()
        {
            string theProfileName = "something";

            IContainer container = new Container(registry =>
            {
                registry.Profile(theProfileName, x =>
                {
                    x.For<IWidget>().Use(() => new AWidget());
                    x.For<Rule>().Use(() => new DefaultRule());
                });
            });

            container.SetDefaultsToProfile(theProfileName);

            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<Rule>();
        }


        [Test]
        public void Add_default_instance_with_concrete_type()
        {
            string theProfileName = "something";

            IContainer container = new Container(registry =>
            {
                registry.Profile(theProfileName, p =>
                {
                    p.For<IWidget>().Use<AWidget>();
                    p.For<Rule>().Use<DefaultRule>();
                });

            });
            container.SetDefaultsToProfile(theProfileName);

            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
            container.GetInstance<Rule>().ShouldBeOfType<DefaultRule>();
        }

        [Test]
        public void Add_default_instance_with_literal()
        {
            var registry = new Registry();
            var theWidget = new AWidget();

            string theProfileName = "something";
            registry.Profile(theProfileName, p => {
                p.For<IWidget>().Use(theWidget);
            });
                

            PluginGraph graph = registry.Build();
            graph.Profile("something").Families[typeof (IWidget)].GetDefaultInstance()
                                                                 .ShouldBeOfType<ObjectInstance>()
                                                                 .Object.ShouldBeTheSameAs(theWidget);
        }

        public class NamedWidget : IWidget
        {
            private readonly string _name;

            public void DoSomething()
            {
                throw new System.NotImplementedException();
            }

            public NamedWidget(string name)
            {
                _name = name;
            }

            public string Name
            {
                get { return _name; }
            }
        }

        [Test]
        public void AddAProfileWithANamedDefault()
        {
            string theProfileName = "TheProfile";
            string theDefaultName = "TheDefaultName";

            var registry = new Registry();

            registry.For<IWidget>().Add(new NamedWidget(theDefaultName)).Named(theDefaultName);
            registry.For<IWidget>().Use<AWidget>();

            registry.Profile(theProfileName, p => {
                p.For<IWidget>().Use(theDefaultName);
                p.For<Rule>().Use("DefaultRule");
            });

            var container = new Container(registry);
            container.SetDefaultsToProfile(theProfileName);
            container.GetInstance<IWidget>().ShouldBeOfType<NamedWidget>()
                     .Name.ShouldEqual(theDefaultName);
        }

        [Test]
        public void AddAProfileWithInlineInstanceDefinition()
        {
            string theProfileName = "TheProfile";

            var container = new Container(registry => {
                registry.For<IWidget>().Use(new NamedWidget("default"));

                registry.Profile(theProfileName, x =>
                {
                    x.For<IWidget>().Use<AWidget>();
                });
            });

            container.SetDefaultsToProfile(theProfileName);
            container.GetInstance<IWidget>().ShouldBeOfType<AWidget>();
        }
    }
}