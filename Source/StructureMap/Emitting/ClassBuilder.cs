using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Emits the IL for a new class Type
    /// </summary>
    public class ClassBuilder
    {
        private const TypeAttributes PUBLIC_ATTS =
            TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.BeforeFieldInit;

        private readonly string _className;

        private readonly ArrayList _methods;
        private readonly TypeBuilder _newTypeBuilder;
        private readonly Type _superType;

        public ClassBuilder(ModuleBuilder module, string className) : this(module, className, typeof (Object))
        {
        }

        public ClassBuilder(ModuleBuilder module, string ClassName, Type superType)
        {
            try
            {
                _methods = new ArrayList();

                _newTypeBuilder = module.DefineType(ClassName, PUBLIC_ATTS, superType);
                _superType = superType;
                _className = ClassName;

                addDefaultConstructor();
            }
            catch (Exception e)
            {
                string message = string.Format("Error trying to emit an InstanceBuilder for {0}", ClassName);
                throw new ApplicationException(message, e);
            }
        }

        public string ClassName
        {
            get { return _className; }
        }


        public void AddMethod(Method method)
        {
            _methods.Add(method);
            method.Attach(_newTypeBuilder);
        }


        internal void Bake()
        {
            foreach (Method method in _methods)
            {
                method.Build();
            }

            _newTypeBuilder.CreateType();
        }


        private void addDefaultConstructor()
        {
            MethodAttributes atts = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig |
                                    MethodAttributes.RTSpecialName;

            ConstructorBuilder construct = _newTypeBuilder.DefineConstructor(atts, CallingConventions.Standard, null);

            ILGenerator ilgen = construct.GetILGenerator();

            ilgen.Emit(OpCodes.Ldarg_0);


            ConstructorInfo constructor = _superType.GetConstructor(new Type[0]);
            ilgen.Emit(OpCodes.Call, constructor);
            ilgen.Emit(OpCodes.Ret);
        }


        public void AddReadonlyStringProperty(string PropertyName, string Value, bool Override)
        {
            PropertyBuilder prop =
                _newTypeBuilder.DefineProperty(PropertyName, PropertyAttributes.HasDefault, typeof (string), null);

            MethodAttributes atts = MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig |
                                    MethodAttributes.Final | MethodAttributes.SpecialName;

            string _GetMethodName = "get_" + PropertyName;

            MethodBuilder methodGet =
                _newTypeBuilder.DefineMethod(_GetMethodName, atts, CallingConventions.Standard, typeof (string), null);
            ILGenerator gen = methodGet.GetILGenerator();

            LocalBuilder ilReturn = gen.DeclareLocal(typeof (string));

            gen.Emit(OpCodes.Ldstr, Value);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ret);

            prop.SetGetMethod(methodGet);
        }
    }
}