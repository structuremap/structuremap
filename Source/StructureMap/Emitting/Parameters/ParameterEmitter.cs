using System;
using System.Reflection;
using System.Reflection.Emit;

namespace StructureMap.Emitting.Parameters
{
	/// <summary>
	/// Abstract class to provide a template for emitting the instructions for retrieving
	/// one constructor function argument from an InstanceMemento and feeding into the 
	/// constructor function
	/// </summary>
	public abstract class ParameterEmitter
	{
		private ParameterEmitter _NextSibling;

		protected ParameterEmitter NextSibling
		{
			set { _NextSibling = value; }
			get { return _NextSibling; }
		}

		public void Generate(ILGenerator ilgen, ParameterInfo parameter)
		{
			if (this.canProcess(parameter.ParameterType))
			{
				this.generate(ilgen, parameter);
			}
			else if (_NextSibling != null)
			{
				_NextSibling.Generate(ilgen, parameter);
			}
			else
			{
				string msg = string.Format("Cannot emit constructor injection for type *{0}*", parameter.ParameterType.AssemblyQualifiedName);
				throw new ApplicationException(msg);
			}
		}

		public void GenerateSetter(ILGenerator ilgen, PropertyInfo property)
		{
			if (this.canProcess(property.PropertyType))
			{
				this.generateSetter(ilgen, property);
			}
			else if (_NextSibling != null)
			{
				_NextSibling.GenerateSetter(ilgen, property);
			}
			else
			{
				string msg = string.Format("Cannot emit constructor injection for type *{0}*", property.PropertyType.AssemblyQualifiedName);
				throw new ApplicationException(msg);
			}
		}

		public void AttachNextSibling(ParameterEmitter Sibling)
		{
			if (this.NextSibling == null)
			{
				this.NextSibling = Sibling;
			}
			else
			{
				this.NextSibling.AttachNextSibling(Sibling);
			}
		}

		protected abstract bool canProcess(Type parameterType);
		protected abstract void generate(ILGenerator ilgen, ParameterInfo parameter);

		protected void callInstanceMemento(ILGenerator ilgen, string MethodName)
		{
			MethodInfo _method = typeof (InstanceMemento).GetMethod(MethodName);
			ilgen.Emit(OpCodes.Callvirt, _method);
		}

		protected void callGetInstanceManager(ILGenerator ilgen)
		{
			PropertyInfo property = (typeof (InstanceBuilder)).GetProperty("Manager");
			MethodInfo methodGetInstanceManager = property.GetGetMethod();
			ilgen.Emit(OpCodes.Call, methodGetInstanceManager);
		}

		protected void cast(ILGenerator ilgen, Type parameterType)
		{
			ilgen.Emit(OpCodes.Castclass, parameterType);
		}

		protected abstract void generateSetter(ILGenerator ilgen, PropertyInfo property);
	}
}