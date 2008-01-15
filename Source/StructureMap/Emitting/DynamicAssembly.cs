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
        private Hashtable _Classes;
        private bool _isCompiled = false;
        private string _Name;
        private AssemblyBuilder assemBuilder;
        private string DLLName;
        private ModuleBuilder module;

        public DynamicAssembly(string Name)
        {
            _Name = Name;
            _Classes = new Hashtable();

            Init();
        }


        public string Name
        {
            get { return _Name; }
        }

        public bool IsCompiled
        {
            get { return _isCompiled; }
        }

        private void Init()
        {
            AssemblyName assemName = new AssemblyName();
            assemName.Name = Name;
            assemName.Version = new Version(1, 0, 0, 0);
            assemName.CodeBase = Name;
            assemName.CultureInfo = new CultureInfo("en");
            assemName.SetPublicKeyToken(null);

            DLLName = _Name + ".DLL";
            //assemBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndSave);
            assemBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);

            //module = assemBuilder.DefineDynamicModule(this.Name, DLLName);
            module = assemBuilder.DefineDynamicModule(Name);
        }


        public ClassBuilder AddClass(string ClassName)
        {
            ClassBuilder newClass = new ClassBuilder(module, ClassName);
            storeClass(newClass);
            return newClass;
        }

        public ClassBuilder AddClass(string ClassName, Type superType)
        {
            ClassBuilder newClass = new ClassBuilder(module, ClassName, superType);
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

            //assemBuilder.Save(DLLName);
            //Assembly assem = AppDomain.CurrentDomain.Load(this.Name);
            _isCompiled = true;
            return (Assembly) assemBuilder;
            //return assem;
        }
    }
}