using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Acceptance
{
    [TestFixture]
    public class builder_for_open_generic_type
    {
        [Test]
        public void show_the_workaround_for_generic_builders()
        {
            var container = new Container(_ => { _.For(typeof (IRepository<,>)).Use(new RepositoryInstanceFactory()); });

            container.GetInstance<IRepository<string, int>>()
                .ShouldBeOfType<Repository<string, int>>();
        }
    }

    public class RepositoryInstanceFactory : Instance
    {
        public override Instance CloseType(Type[] types)
        {
            // StructureMap will cache the object built out of this,
            // so the expensive Reflection hit only happens
            // once
            var instanceType = typeof (RepositoryInstance<,>).MakeGenericType(types);
            return Activator.CreateInstance(instanceType).As<Instance>();
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotSupportedException();
        }

        public override string Description
        {
            get { return "Build Repository<T, T1>() with RepositoryBuilder"; }
        }

        public override Type ReturnedType
        {
            get { return typeof (Repository<,>); }
        }
    }

    public class RepositoryInstance<T, T1> : LambdaInstance<IRepository<T, T1>>
    {
        public RepositoryInstance() : base(() => RepositoryBuilder.Build<T, T1>())
        {
        }
    }

    public static class RepositoryBuilder
    {
        public static IRepository<T, T1> Build<T, T1>()
        {
            return new Repository<T, T1>();
        }
    }

    public interface IRepository<T, T1>
    {
    }

    public class Repository<T, T1> : IRepository<T, T1>
    {
    }
}