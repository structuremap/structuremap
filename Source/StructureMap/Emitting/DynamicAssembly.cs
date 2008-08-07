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
        private string _name;
        private AssemblyBuilder _assemblyBuilder;
        private string DLLName;
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
            AssemblyName assemName = new AssemblyName();
            assemName.Name = Name;
            assemName.Version = new Version(1, 0, 0, 0);
            assemName.CodeBase = Name;
            assemName.CultureInfo = new CultureInfo("en");
            assemName.SetPublicKeyToken(null);

            DLLName = Name + ".dll";
            _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.RunAndSave);
            //_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(assemName, AssemblyBuilderAccess.Run);

            _module = _assemblyBuilder.DefineDynamicModule(this.Name, DLLName);
            
            
            //_module = _assemblyBuilder.DefineDynamicModule(Name);
        }


        public ClassBuilder AddClass(string ClassName)
        {
            ClassBuilder newClass = new ClassBuilder(_module, ClassName);
            storeClass(newClass);
            return newClass;
        }

        public ClassBuilder AddClass(string ClassName, Type superType)
        {
            ClassBuilder newClass = new ClassBuilder(_module, ClassName, superType);
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

            _assemblyBuilder.Save(_name + ".dll");

            //assemBuilder.Save(DLLName);
            //Assembly assem = AppDomain.CurrentDomain.Load(this.Name);
            _isCompiled = true;
            return (Assembly) _assemblyBuilder;
            //return assem;
        }
    }
}