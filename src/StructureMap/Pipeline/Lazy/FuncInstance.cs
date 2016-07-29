using System;

namespace StructureMap.Pipeline.Lazy
{
#pragma warning disable 1591

    // SAMPLE: FuncInstance
    public class FuncInstance<T> : LambdaInstance<Func<T>>
    {
        // Pass a Func<IContext, T> into the base constructor
        // Use a static method for cleaner syntax to avoid the 
        // nasty Lambda syntax formatting as needed
        public FuncInstance() : base(s => s.GetInstance<IContainer>().GetInstance<T>)
        {
        }

        public override string Description
        {
            get { return "Constructor for Func<{0}>".ToFormat(typeof (T).Name); }
        }
    }
    // ENDSAMPLE

#pragma warning restore 1591
}