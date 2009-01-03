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

        public BuildInstanceMethod(Plugin plugin)
        {
            _plugin = plugin;
        }


        public override Type[] ArgumentList
        {
            get { return new[] {typeof (IConfiguredInstance), typeof (BuildSession)}; }
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
            ilgen.Emit(OpCodes.Nop);
            ilgen.DeclareLocal(_plugin.PluggedType);
            ilgen.DeclareLocal(typeof (object));

            for (int i = 0; i < _plugin.Setters.OptionalCount; i++)
            {
                ilgen.DeclareLocal(typeof (bool));
            }

            var arguments = new ArgumentEmitter(ilgen, true);

            _plugin.VisitConstructor(arguments);

            ilgen.Emit(OpCodes.Newobj, _plugin.GetConstructor());
            Label label = ilgen.DefineLabel();
            ilgen.Emit(OpCodes.Stloc_0);

            _plugin.VisitSetters(arguments);

            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Stloc_1);

            ilgen.Emit(OpCodes.Br_S, label);
            ilgen.MarkLabel(label);
            ilgen.Emit(OpCodes.Ldloc_1);
            ilgen.Emit(OpCodes.Ret);
        }
    }
}