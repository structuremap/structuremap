using System;

namespace StructureMap.Pipeline.Lazy
{
    public class LazyInstance<T> : LambdaInstance<Func<T>>
    {
        public LazyInstance() : base(s => s.GetInstance<IContainer>().GetInstance<T>)
        {
        }

        protected override string getDescription()
        {
            return "Constructor for Func<{0}>".ToFormat(typeof(T).Name);
        }
    }
}