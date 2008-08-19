using System;
using NUnit.Framework;
using StructureMap.Configuration.DSL;
using StructureMap.Diagnostics;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Diagnostics
{
    [TestFixture]
    public class ValidationBuildSessionTester : Registry
    {
        private ValidationBuildSession validatedSession(Action<Registry> action)
        {
            Registry registry = new Registry();
            action(registry);

            PluginGraph graph = registry.Build();
            ValidationBuildSession session = new ValidationBuildSession(graph);
            session.PerformValidations();

            return session;
        }


        private BuildError getFirstAndOnlyError(ValidationBuildSession session)
        {
            Assert.AreEqual(1, session.BuildErrors.Length);
            return session.BuildErrors[0];
        }

        private ConstructorInstance<IWidget> errorInstance()
        {
            return ConstructedBy<IWidget>(delegate { throw new NotSupportedException("You can't make me!"); });
        }

        [Test]
        public void Attach_dependency_to_the_build_error_but_do_not_create_new_error_for_dependency()
        {
            ValidationBuildSession session = validatedSession(registry =>
            {
                registry.AddInstanceOf<IWidget>(errorInstance().WithName("BadInstance"));

                registry.AddInstanceOf<SomethingThatNeedsAWidget>(
                    Instance<SomethingThatNeedsAWidget>().WithName("DependentInstance")
                        .Child<IWidget>().IsNamedInstance("BadInstance")
                    );
            });

            BuildError error = getFirstAndOnlyError(session);

            Assert.AreEqual(1, error.Dependencies.Count);
            BuildDependency dependency = error.Dependencies[0];
            Assert.AreEqual(typeof (SomethingThatNeedsAWidget), dependency.PluginType);
            Assert.AreEqual("DependentInstance", dependency.Instance.Name);
        }

        [Test]
        public void Create_an_instance_for_the_first_time_happy_path()
        {
            ValidationBuildSession session =
                validatedSession(
                    registry => registry.AddInstanceOf<IWidget>(new ColorWidget("Red")));

            Assert.AreEqual(0, session.BuildErrors.Length);
        }

        [Test]
        public void Create_an_instance_that_fails_and_an_instance_that_depends_on_that_exception()
        {
            ValidationBuildSession session = validatedSession(registry =>
            {
                registry.AddInstanceOf<IWidget>(errorInstance().WithName("BadInstance"));

                registry.AddInstanceOf<SomethingThatNeedsAWidget>(
                    Instance<SomethingThatNeedsAWidget>().WithName("DependentInstance")
                        .Child<IWidget>().IsNamedInstance("BadInstance")
                    );
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
                registry => registry.AddInstanceOf<SomethingThatNeedsAWidget>(
                                Instance<SomethingThatNeedsAWidget>().WithName("BadInstance")
                                    .Child<IWidget>().Is(errorInstance())
                                ));

            BuildError error = getFirstAndOnlyError(session);

            Assert.AreEqual("BadInstance", error.Instance.Name);
            Assert.AreEqual(typeof (SomethingThatNeedsAWidget), error.PluginType);
        }

        [Test]
        public void Create_an_instance_that_has_a_build_failure()
        {
            Instance instance = errorInstance().WithName("Bad");
            ValidationBuildSession session =
                validatedSession(registry => registry.AddInstanceOf<IWidget>(instance));

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
                    registry => registry.AddInstanceOf<IWidget>(new ColorWidget("Red")));

            Assert.AreEqual(0, session.ValidationErrors.Length);
        }

        [Test]
        public void Request_an_instance_for_the_second_time_successfully_and_get_the_same_object()
        {
            ValidationBuildSession session = new ValidationBuildSession(new PluginGraph());

            LiteralInstance instance = Object(new ColorWidget("Red"));
            object widget1 = session.CreateInstance(typeof (IWidget), instance);
            object widget2 = session.CreateInstance(typeof (IWidget), instance);

            Assert.AreSame(widget1, widget2);
        }

        [Test]
        public void Successfully_build_an_object_that_has_multiple_validation_errors()
        {
            ValidationBuildSession session =
                validatedSession(
                    registry => registry.BuildInstancesOf<SomethingThatHasValidationFailures>());

            Assert.AreEqual(2, session.ValidationErrors.Length);
        }

        [Test]
        public void Validate_a_single_object_with_both_a_passing_validation_method_and_a_failing_validation_method()
        {
            LiteralInstance instance = new LiteralInstance(new WidgetWithOneValidationFailure());
            ValidationBuildSession session =
                validatedSession(registry => registry.AddInstanceOf<IWidget>(instance));


            Assert.AreEqual(1, session.ValidationErrors.Length);

            ValidationError error = session.ValidationErrors[0];

            Assert.AreEqual(typeof (IWidget), error.PluginType);
            Assert.AreEqual(instance, error.Instance);
            Assert.AreEqual("ValidateFailure", error.MethodName);
            Assert.IsInstanceOfType(typeof (NotImplementedException), error.Exception);
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