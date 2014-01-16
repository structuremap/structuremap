using System;

namespace StructureMap.Pipeline.Lazy
{
    public class LazyInstance<T> : LambdaInstance<Lazy<T>>
    {
        public LazyInstance()
            : base(s => new Lazy<T>(() => s.GetInstance<IContainer>().GetInstance<T>()))
        {
        }

        public override string Description
        {
            get { return "Constructor for Lazy<{0}>".ToFormat(typeof(T).Name); }
        }
    }
}