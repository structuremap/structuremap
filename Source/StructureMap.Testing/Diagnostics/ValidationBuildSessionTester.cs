using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class ValidationBuildSessionTester : Registry
    {
        private ValidationBuildSession validatedSession(Action<Registry> action)
        {
            var registry = new Registry();
            action(registry);

            PluginGraph graph = registry.Build();
            var session = new ValidationBuildSession(graph);
            session.PerformValidations();

            return session;
        }


        private BuildError getFirstAndOnlyError(ValidationBuildSession session)
        {
            Assert.AreEqual(1, session.BuildErrors.Length);
            return session.BuildErrors[0];
        }

        private LambdaInstance<IWidget> errorInstance()
        {
            return
                new LambdaInstance<IWidget>(delegate() { throw new NotSupportedException("You can't make me!"); });
        }

        [Test]
        public void Attach_dependency_to_the_build_error_but_do_not_create_new_error_for_dependency()
        {
            ValidationBuildSession session = validatedSession(r =>
            {
                r.InstanceOf<IWidget>().IsThis(errorInstance().WithName("BadInstance"));

                r.InstanceOf<SomethingThatNeedsAWidget>().Is.OfConcreteType<SomethingThatNeedsAWidget>()
                    .WithName("DependentInstance")
                    .CtorDependency<IWidget>().Is(x => x.TheInstanceNamed("BadInstance"));
            });

            BuildError error = getFirstAndOnlyError(session);

            Assert.AreEqual(1, error.Dependencies.Count);
            BuildDependency dependency = error.Dependencies[0];
            Assert.AreEqual(typeof (SomethingThatNeedsAWidget), dependency.PluginType);
            Assert.AreEqual("DependentInstance", dependency.Instance.Name);
        }

        [Test]
        public void do_not_fail_with_the_bidirectional_checks()
        {
            var container = new Container(r =>
            {
                r.For<IWidget>().Use<ColorWidget>().WithCtorArg("color").EqualTo("red");
                r.For<Rule>().Use<WidgetRule>();

                r.ForConcreteType<ClassThatNeedsWidgetAndRule1>();
                r.ForConcreteType<ClassThatNeedsWidgetAndRule2>();
                r.InstanceOf<ClassThatNeedsWidgetAndRule2>().Is.OfConcreteType<ClassThatNeedsWidgetAndRule2>();
                r.InstanceOf<ClassThatNeedsWidgetAndRule2>().Is.OfConcreteType<ClassThatNeedsWidgetAndRule2>();
                r.InstanceOf<ClassThatNeedsWidgetAndRule2>().Is.OfConcreteType<ClassThatNeedsWidgetAndRule2>();
                r.InstanceOf<ClassThatNeedsWidgetAndRule2>().Is.OfConcreteType<ClassThatNeedsWidgetAndRule2>().CtorDependency<Rule>().Is<ARule>();


            });

            container.AssertConfigurationIsValid();
        }


        [Test]
        public void Create_an_instance_for_the_first_time_happy_path()
        {
            ValidationBuildSession session =
                validatedSession(
                    r => r.InstanceOf<IWidget>().Is.Object(new ColorWidget("Red")));

            Assert.AreEqual(0, session.BuildErrors.Length);
        }

        [Test]
        public void Create_an_instance_that_fails_and_an_instance_that_depends_on_that_exception()
        {
            ValidationBuildSession session = validatedSession(r =>
            {
                r.InstanceOf<IWidget>().IsThis(errorInstance().WithName("BadInstance"));


                r.InstanceOf<SomethingThatNeedsAWidget>().Is.OfConcreteType<SomethingThatNeedsAWidget>()
                    .WithName("DependentInstance")
                    .CtorDependency<IWidget>().Is(x => x.TheInstanceNamed("BadInstance"));
            });

            Assert.AreEqual(1, session.BuildErrors.Length);

            BuildError error = session.Find(typeof (SomethingThatNeedsAWidget), "DependentInstance");
            Assert.IsNull(error);

            BuildError error2 = session.Find(typeof (IWidget), "BadInstance");
            Assert.IsNotNull(error2);
        }

        [Test]
        public void Create_an_instance_that_fails_because_of_an_inline_child()
        {
            ValidationBuildSession session = validatedSession(
                r =>
                {
                    r.InstanceOf<SomethingThatNeedsAWidget>().Is.OfConcreteType<SomethingThatNeedsAWidget>()
                        .WithName("BadInstance")
                        .CtorDependency<IWidget>().Is(errorInstance());
                });

            BuildError error = getFirstAndOnlyError(session);

            Assert.AreEqual("BadInstance", error.Instance.Name);
            Assert.AreEqual(typeof (SomethingThatNeedsAWidget), error.PluginType);
        }

        [Test]
        public void Create_an_instance_that_has_a_build_failure()
        {
            Instance instance = errorInstance().WithName("Bad");
            ValidationBuildSession session =
                validatedSession(registry => registry.InstanceOf<IWidget>().IsThis(instance));

            BuildError error = getFirstAndOnlyError(session);

            Assert.AreEqual(instance, error.Instance);
            Assert.AreEqual(typeof (IWidget), error.PluginType);
            Assert.IsNotNull(error.Exception);
        }

        [Test]
        public void Happy_path_with_no_validation_errors()
        {
            ValidationBuildSession session =
                validatedSession(
                    registry => registry.InstanceOf<IWidget>().Is.Object(new ColorWidget("Red")));

            Assert.AreEqual(0, session.ValidationErrors.Length);
        }

        [Test]
        public void Request_an_instance_for_the_second_time_successfully_and_get_the_same_object()
        {
            var session = new ValidationBuildSession(new PluginGraph());

            var instance = new ObjectInstance(new ColorWidget("Red"));
            object widget1 = session.CreateInstance(typeof (IWidget), instance);
            object widget2 = session.CreateInstance(typeof (IWidget), instance);

            Assert.AreSame(widget1, widget2);
        }

        [Test]
        public void Successfully_build_an_object_that_has_multiple_validation_errors()
        {
            ValidationBuildSession session =
                validatedSession(
                    registry => registry.BuildInstancesOf<SomethingThatHasValidationFailures>().TheDefaultIsConcreteType<SomethingThatHasValidationFailures>());

            Assert.AreEqual(2, session.ValidationErrors.Length);
        }

        [Test]
        public void Validate_a_single_object_with_both_a_passing_validation_method_and_a_failing_validation_method()
        {
            var instance = new ObjectInstance(new WidgetWithOneValidationFailure());
            ValidationBuildSession session =
                validatedSession(registry => registry.InstanceOf<IWidget>().IsThis(instance));


            Assert.AreEqual(1, session.ValidationErrors.Length);

            ValidationError error = session.ValidationErrors[0];

            Assert.AreEqual(typeof (IWidget), error.PluginType);
            Assert.AreEqual(instance, error.Instance);
            Assert.AreEqual("ValidateFailure", error.MethodName);
            Assert.IsInstanceOfType(typeof (NotImplementedException), error.Exception);
        }
    }

    public class ClassThatNeedsWidgetAndRule1
    {
        public ClassThatNeedsWidgetAndRule1(IWidget widget, Rule rule)
        {
        }
    }

    public class ClassThatNeedsWidgetAndRule2
    {
        public ClassThatNeedsWidgetAndRule2(IWidget widget, Rule rule, ClassThatNeedsWidgetAndRule1 class1)
        {
        }
    }

    public class WidgetWithOneValidationFailure : IWidget
    {
        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion

        [ValidationMethod]
        public void ValidateSuccess()
        {
            // everything is wonderful
        }

        [ValidationMethod]
        public void ValidateFailure()
        {
            throw new NotImplementedException("I'm not ready");
        }
    }

    public class SomethingThatHasValidationFailures
    {
        [ValidationMethod]
        public void Validate1()
        {
            throw new NotSupportedException("You can't make me");
        }

        [ValidationMethod]
        public void Validate2()
        {
            throw new NotImplementedException("Not ready yet");
        }
    }

    public class SomethingThatNeedsAWidget
    {
        public SomethingThatNeedsAWidget(IWidget widget)
        {
        }
    }
}