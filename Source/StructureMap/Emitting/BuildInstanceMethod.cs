using System;
using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Emitting.Parameters;
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
        private readonly ConstructorInfo _constructor;
        private readonly ParameterEmitter _parameterEmitter;

        public BuildInstanceMethod(Plugin plugin) : base()
        {
            _constructor = plugin.GetConstructor();

            _parameterEmitter = new StringParameterEmitter();

            _parameterEmitter.AttachNextSibling(new PrimitiveParameterEmitter());
            _parameterEmitter.AttachNextSibling(new EnumParameterEmitter());
            _parameterEmitter.AttachNextSibling(new ChildParameterEmitter());
            _parameterEmitter.AttachNextSibling(new ChildArrayParameterEmitter());
            _plugin = plugin;
        }


        public override Type[] ArgumentList
        {
            get { return new Type[] { typeof(IConfiguredInstance), typeof(StructureMap.Pipeline.IInstanceCreator) }; }
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

            foreach (ParameterInfo parameter in _constructor.GetParameters())
            {
                _parameterEmitter.Generate(ilgen, parameter);
            }

            ilgen.Emit(OpCodes.Newobj, _constructor);
            Label label = ilgen.DefineLabel();
            ilgen.Emit(OpCodes.Stloc_0);

            foreach (SetterProperty setter in _plugin.Setters)
            {
                _parameterEmitter.GenerateSetter(ilgen, setter.Property);
            }

            ilgen.Emit(OpCodes.Br_S, label);
            ilgen.MarkLabel(label);
            ilgen.Emit(OpCodes.Ldloc_0);
            ilgen.Emit(OpCodes.Ret);
        }
    }
}