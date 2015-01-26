using System;
using System.Diagnostics;
using NUnit.Framework;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class AssertConfigurationIsValid_Smoke_Tester
    {
        [Test]
        public void happy_path_with_build_plans_all_good()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<NamedWidget>().Ctor<string>().Is("Marshall");
            });

            container.AssertConfigurationIsValid();
        }

        [Test]
        public void sad_path_with_an_invalid_build_plan()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<NamedWidget>();
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  1 Build/Configuration Failures and 0 Validation Errors");
            ex.Context.ShouldContain("Unable to create a build plan for concrete type StructureMap.Testing.Diagnostics.NamedWidget");

        }

        [Test]
        public void happy_path_with_validation_method()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<ValidatingFailureWidget>().Ctor<bool>("fails").Is(false);
            });

            container.AssertConfigurationIsValid();
        }

        [Test]
        public void sad_path_with_validation_method()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<ValidatingFailureWidget>().Ctor<bool>("fails").Is(true);
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() => {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  0 Build/Configuration Failures and 1 Validation Errors");
            ex.Context.ShouldContain("Validation Error in Method Validate");
        }

        [Test]
        public void sad_path_with_ctor_failure()
        {
            var container = new Container(x => {
                x.For<IWidget>().Use<FailingWidget>();
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  1 Build/Configuration Failures and 0 Validation Errors");

            Debug.WriteLine(ex.Title);
            Debug.WriteLine(ex.Context);
        }

        [Test]
        public void only_registers_the_root_cause_for_build_problems()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<FailingWidget>();
                x.ForConcreteType<WidgetHolder>();
                x.ForConcreteType<WidgetHolderHolder>();
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  1 Build/Configuration Failures and 0 Validation Errors");

            Debug.WriteLine(ex.Title);
            Debug.WriteLine(ex.Context);
        }


        [Test]
        public void sad_path_with_validation_method_in_profile()
        {
            var container = new Container(x =>
            {
                x.For<IWidget>().Use<ValidatingFailureWidget>().Ctor<bool>("fails").Is(false);

                x.Profile("Blue", blue => {
                    x.For<IWidget>().Use<ValidatingFailureWidget>().Ctor<bool>("fails").Is(true);
                });

                
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  0 Build/Configuration Failures and 1 Validation Errors");
            ex.Context.ShouldContain("Validation Error in Method Validate");
        }

        [Test]
        public void missing_default()
        {
            var container = new Container(x =>
            {
                x.ForConcreteType<WidgetHolder>();
                x.ForConcreteType<WidgetHolderHolder>();
            });

            var ex = Exception<StructureMapConfigurationException>.ShouldBeThrownBy(() =>
            {
                container.AssertConfigurationIsValid();
            });

            ex.Title.ShouldEqual("StructureMap Failures:  1 Build/Configuration Failures and 0 Validation Errors");

            Debug.WriteLine(ex.Title);
            Debug.WriteLine(ex.Context);
        }
    }

    public interface IWidget{}

    public class WidgetHolderHolder
    {
        public WidgetHolderHolder(WidgetHolder holder)
        {
        }
    }

    public class WidgetHolder
    {
        public WidgetHolder(IWidget widget)
        {
        }
    }

    public class NamedWidget : IWidget
    {
        public NamedWidget(string name)
        {
        }

        public void DoSomething()
        {
            
        }
    }

    public class FailingWidget : IWidget
    {
        public FailingWidget()
        {
            throw new DivideByZeroException("No soup for you!");
        }

        public void DoSomething()
        {
        }
    }

    public class ValidatingFailureWidget : IWidget
    {
        private readonly bool _fails;

        public ValidatingFailureWidget(bool fails)
        {
            _fails = fails;
        }

        public void DoSomething()
        {
            
        }

        [ValidationMethod]
        public void Validate()
        {
            if (_fails)
            {
                throw new Exception("I failed with a validation exception");
            }
        }
    }

}