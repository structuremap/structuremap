using System;
using System.Reflection;
using System.Reflection.Emit;
using StructureMap.Pipeline;

namespace StructureMap.Emitting.Parameters
{
    /// <summary>
    /// Abstract class to provide a template for emitting the instructions for retrieving
    /// one constructor function argument from an InstanceMemento and feeding into the 
    /// constructor function
    /// </summary>
    public abstract class ParameterEmitter
    {
        private ParameterEmitter _nextSibling;

        protected ParameterEmitter NextSibling
        {
            set { _nextSibling = value; }
            get { return _nextSibling; }
        }

        public void Generate(ILGenerator ilgen, ParameterInfo parameter)
        {
            if (canProcess(parameter.ParameterType))
            {
                generate(ilgen, parameter);
            }
            else if (_nextSibling != null)
            {
                _nextSibling.Generate(ilgen, parameter);
            }
            else
            {
                string msg =
                    string.Format("Cannot emit constructor injection for type *{0}*",
                                  parameter.ParameterType.AssemblyQualifiedName);
                throw new ApplicationException(msg);
            }
        }

        public void GenerateSetter(ILGenerator ilgen, PropertyInfo property)
        {
            if (canProcess(property.PropertyType))
            {
                generateSetter(ilgen, property);
            }
            else if (_nextSibling != null)
            {
                _nextSibling.GenerateSetter(ilgen, property);
            }
            else
            {
                string msg =
                    string.Format("Cannot emit constructor injection for type *{0}*",
                                  property.PropertyType.AssemblyQualifiedName);
                throw new ApplicationException(msg);
            }
        }

        public void AttachNextSibling(ParameterEmitter Sibling)
        {
            if (NextSibling == null)
            {
                NextSibling = Sibling;
            }
            else
            {
                NextSibling.AttachNextSibling(Sibling);
            }
        }

        protected abstract bool canProcess(Type parameterType);
        protected abstract void generate(ILGenerator ilgen, ParameterInfo parameter);

        protected void callInstanceMemento(ILGenerator ilgen, string methodName)
        {
            MethodInfo _method = typeof (IConfiguredInstance).GetMethod(methodName);
            ilgen.Emit(OpCodes.Callvirt, _method);
        }

        protected void cast(ILGenerator ilgen, Type parameterType)
        {
            ilgen.Emit(OpCodes.Castclass, parameterType);
        }

        protected abstract void generateSetter(ILGenerator ilgen, PropertyInfo property);
    }
}