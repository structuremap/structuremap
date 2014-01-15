using System;
using StructureMap.Building;

namespace StructureMap.Pipeline
{
    public class FactoryTemplate : Instance
    {
        private readonly Type _openInstanceType;

        public FactoryTemplate(Type openInstanceType)
        {
            _openInstanceType = openInstanceType;
        }

        protected override string getDescription()
        {
            return string.Empty;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }

        public override IDependencySource ToBuilder(Type pluginType, Policies policies)
        {
            return base.ToBuilder(pluginType, policies);
        }

        public override Instance CloseType(Type[] types)
        {
            var instanceType = _openInstanceType.MakeGenericType(types);
            return (Instance) Activator.CreateInstance(instanceType);
        }
    }


    public class LazyInstance<T> : Instance
    {
        protected override string getDescription()
        {
            return "Lazy construction of " + typeof (T).FullName;
        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }


//        protected override object build(Type pluginType, IBuildSession session)
//        {
//            var container = session.GetInstance<IContainer>();
//            Func<T> func = container.GetInstance<T>;
//
//            return func;
//        }
    }

    public class FactoryInstance<T> : Instance
    {
        protected override string getDescription()
        {
            return "Lazy factory of " + typeof (T).FullName;
        }

//        protected override object build(Type pluginType, IBuildSession session)
//        {
//            var container = session.GetInstance<IContainer>();
//            Func<string, T> func = container.GetInstance<T>;
//
//            return func;
//        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }
    }

    public class FactoryInstance<T, T1> : Instance
    {
        protected override string getDescription()
        {
            return "Lazy construction of {0} using {1}".ToFormat(typeof (T1).FullName, typeof (T).FullName);
        }

//        protected override object build(Type pluginType, IBuildSession session)
//        {
//            var container = session.GetInstance<IContainer>();
//            Func<T, T1> func = key => container.With(key).GetInstance<T1>();
//
//            return func;
//        }

        public override IDependencySource ToDependencySource(Type pluginType)
        {
            throw new NotImplementedException();
        }

    }
}