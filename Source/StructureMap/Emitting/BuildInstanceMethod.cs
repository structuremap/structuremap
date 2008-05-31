using System;
using System.Reflection.Emit;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Emits the IL for the InstanceBuilder.BuildInstance(InstanceMemento) method of an
    /// InstanceBuilder
    /// </summary>
    public class BuildInstanceMethod : Method
    {
        private readonly Plugin _plugin;

        public BuildInstanceMethod(Plugin plugin) : base()
        {
            _plugin = plugin;
        }


        public override Type[] ArgumentList
        {
            get { return new Type[] {typeof (IConfiguredInstance), typeof (IBuildSession)}; }
        }

        public override string MethodName
        {
            get { return "BuildInstance"; }
        }


        public override Type ReturnType
        {
            get { return typeof (object); }
        }


        protected override void Generate(ILGenerator ilgen)
        {
            ilgen.DeclareLocal(typeof (object));
            ArgumentEmitter arguments = new ArgumentEmitter(ilgen);

            _plugin.VisitConstructor(arguments);

            ilgen.Emit(OpCodes.Newobj, _plugin.GetConstructor());
            Label label = ilgen.DefineLabel();
            ilgen.Emit(OpCodes.Stloc_0);

            _plugin.VisitSetters(arguments);

            ilgen.Emit(OpCodes.Br_S, label);
            ilgen.MarkLabel(label);
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ret);
        }
    }
}