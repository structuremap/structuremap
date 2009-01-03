using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Emitting
{
    public class BuildUpMethod : Method
    {
        private readonly Plugin _plugin;

        public BuildUpMethod(Plugin plugin)
        {
            _plugin = plugin;
        }

        public override string MethodName
        {
            get { return "BuildUp"; }
        }

        public override Type[] ArgumentList
        {
            get { return new Type[] { typeof(IConfiguredInstance), typeof(BuildSession) , typeof(object)}; }
        }

        public override Type ReturnType
        {
            get { return typeof(void); }
        }

        protected override void Generate(ILGenerator ilgen)
        {
            ilgen.Emit(OpCodes.Nop);

            ilgen.DeclareLocal(_plugin.PluggedType);
            ilgen.DeclareLocal(typeof(bool));

            ilgen.Emit(OpCodes.Ldarg_3);
            ilgen.Emit(OpCodes.Castclass, _plugin.PluggedType);
            ilgen.Emit(OpCodes.Stloc_0);

            var arguments = new ArgumentEmitter(ilgen, false);

            _plugin.VisitSetters(arguments);

            ilgen.Emit(OpCodes.Ret);

        }
    }
}
