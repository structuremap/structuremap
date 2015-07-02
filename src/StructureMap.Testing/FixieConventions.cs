using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fixie;
using FubuCore.Reflection;
using NUnit.Framework;
using StructureMap.Testing;

namespace NUnit.Framework
{
    public class TestFixtureAttribute : Attribute
    {
    }

    public class TestAttribute : Attribute
    {
    }

    public class ExplicitAttribute : Attribute
    {
        private readonly string _description;

        public ExplicitAttribute()
        {
        }

        public ExplicitAttribute(string description)
        {
            _description = description;
        }

        public string description
        {
            get { return _description; }
        }
    }

    public class TestFixtureSetUpAttribute : Attribute
    {
    }

    public class SetUpAttribute : Attribute
    {
    }

    public class TearDownAttribute : Attribute
    {
    }

    public class TestFixtureTearDownAttribute : Attribute
    {
    }

    public static class Assert
    {
        public static void Fail(string message)
        {
            throw new Exception(message);
        }

        public static void AreSame(this object anObject, object other)
        {
            anObject.ShouldBeTheSameAs(other);
        }


        public static void AreNotSame(this object anObject, object other)
        {
            anObject.ShouldNotBeTheSameAs(other);
        }

        public static void IsNull(this object anObject)
        {
            anObject.ShouldBeNull();
        }

        public static void IsNotNull(this object anObject)
        {
            anObject.ShouldNotBeNull();
        }


        public static void NotNull(this object anObject)
        {
            anObject.ShouldNotBeNull();
        }


        public static void IsFalse(this bool condition)
        {
            condition.ShouldBeFalse();
        }

        public static void IsTrue(this bool condition)
        {
            condition.ShouldBeTrue();
        }
    }
}

namespace StructureMap.Testing
{
    public class CustomConvention : Convention
    {
        public CustomConvention()
        {
            Classes
                .HasOrInherits<TestFixtureAttribute>();

            Methods
                .HasOrInherits<TestAttribute>().Where(m => !m.HasAttribute<ExplicitAttribute>());

            ClassExecution
                .CreateInstancePerClass()
                .SortCases((caseA, caseB) => String.Compare(caseA.Name, caseB.Name, StringComparison.Ordinal));

            FixtureExecution
                .Wrap<FixtureSetUpTearDown>();

            CaseExecution
                .Wrap<SetUpTearDown>();
        }
    }


    internal class SetUpTearDown : CaseBehavior
    {
        public void Execute(Case @case, Action next)
        {
            @case.Class.InvokeAll<SetUpAttribute>(@case.Fixture.Instance);
            next();
            @case.Class.InvokeAll<TearDownAttribute>(@case.Fixture.Instance);
        }
    }

    internal class FixtureSetUpTearDown : FixtureBehavior
    {
        public void Execute(Fixture fixture, Action next)
        {
            fixture.Class.Type.InvokeAll<TestFixtureSetUpAttribute>(fixture.Instance);
            next();
            fixture.Class.Type.InvokeAll<TestFixtureTearDownAttribute>(fixture.Instance);
        }
    }

    public static class BehaviorBuilderExtensions
    {
        public static void InvokeAll<TAttribute>(this Type type, object instance)
            where TAttribute : Attribute
        {
            foreach (var method in Has<TAttribute>(type))
            {
                try
                {
                    method.Invoke(instance, null);
                }
                catch (TargetInvocationException exception)
                {
                    throw new PreservedException(exception.InnerException);
                }
            }
        }

        private static IEnumerable<MethodInfo> Has<TAttribute>(Type type) where TAttribute : Attribute
        {
            return type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.HasOrInherits<TAttribute>());
        }
    }
}