using System;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting
{
    /// <summary>
    /// Helper class to use to create a dynamically emitted assembly
    /// </summary>
    public class DynamicAssembly
    {
        private readonly Hashtable _Classes;
        private readonly string _name;
        private AssemblyBuilder _assemblyBuilder;
        private bool _isCompiled;
        private ModuleBuilder _module;

        public DynamicAssembly(string name)
        {
            _name = name;
            _Classes = new Hashtable();

            Init();
        }


        public string Name
        {
            get { return _name; }
        }

        public bool IsCompiled
        {
            get { return _isCompiled; }
        }

        private void Init()
        {
            var assemName = new AssemblyName();
            assemName.Name = Name;
            assemName.Version = new Version(1, 0, 0, 0);
            assemName.CodeBase = Name;
            assemName.CultureInfo = new CultureInfo("en");
            assemName.SetPublicKeyToken(null);

            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);

            _module = _assemblyBuilder.DefineDynamicModule(Name);
        }


        public ClassBuilder AddClass(string ClassName)
        {
            var newClass = new ClassBuilder(_module, ClassName);
            storeClass(newClass);
            return newClass;
        }

        public ClassBuilder AddClass(string ClassName, Type superType)
        {
            var newClass = new ClassBuilder(_module, ClassName, superType);
            storeClass(newClass);
            return newClass;
        }

        private void storeClass(ClassBuilder newClass)
        {
            _Classes.Add(newClass.ClassName, newClass);
        }


        public Assembly Compile()
        {
            foreach (ClassBuilder newClass in _Classes.Values)
            {
                newClass.Bake();
            }

            _isCompiled = true;
            return _assemblyBuilder;
        }
    }
}