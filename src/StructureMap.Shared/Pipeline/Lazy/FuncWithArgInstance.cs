using System;

namespace StructureMap.Pipeline.Lazy
{
    public class FuncWithArgInstance<TIn,T> : LambdaInstance<Func<TIn,T>>
    {
        public FuncWithArgInstance() : base(s => @in => s.GetInstance<IContainer>().With(@in).GetInstance<T>())
        {
        }

        public override string Description
        {
            get { return "Constructor for Func<{0},{1}>".ToFormat(typeof(TIn).Name, typeof (T).Name); }
        }
    }
}