using System;

namespace StructureMap.Pipeline.Lazy
{
    public class FuncInstance<T> : LambdaInstance<Func<T>>
    {
        public FuncInstance() : base(s => s.GetInstance<IContainer>().GetInstance<T>)
        {
        }

        public override string Description
        {
            get { return "Constructor for Func<{0}>".ToFormat(typeof (T).Name); }
        }
    }
}