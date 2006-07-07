using System;
using System.Collections;
using System.Reflection;

namespace StructureMap
{
	/// <summary>
	/// Marks a method with no parameters as a method that validates an instance.  StructureMap
	/// uses this method to validate the configuration file.  If the method does not throw an
	/// exception, the object is assumed to be valid.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ValidationMethodAttribute : Attribute
	{
		public ValidationMethodAttribute()
		{
		}

		/// <summary>
		/// Returns an array of any MethodInfo's on a Type that are marked as ValidationMethod
		/// </summary>
		/// <param name="objectType">CLR Type to search for validation methods</param>
		/// <returns></returns>
		public static MethodInfo[] GetValidationMethods(Type objectType)
		{
			ArrayList methodList = new ArrayList();

			MethodInfo[] methods = objectType.GetMethods();
			foreach (MethodInfo method in methods)
			{
				ValidationMethodAttribute att =
					(ValidationMethodAttribute) Attribute.GetCustomAttribute(method, typeof (ValidationMethodAttribute));

				if (att != null)
				{
					if (method.GetParameters().Length > 0)
					{
						string msg = string.Format("Method *{0}* in Class *{1}* cannot be a validation method because it has parameters",
						                           method.Name, objectType.FullName);
						throw new ApplicationException(msg);
					}

					methodList.Add(method);
				}
			}

			MethodInfo[] returnValue = new MethodInfo[methodList.Count];
			methodList.CopyTo(returnValue, 0);

			return returnValue;
		}


		/// <summary>
		/// Executes the marked validation methods, if any, on an object
		/// </summary>
		/// <param name="target"></param>
		public static void CallValidationMethods(object target)
		{
			MethodInfo[] methods = GetValidationMethods(target.GetType());
			foreach (MethodInfo method in methods)
			{
				method.Invoke(target, new object[0]);
			}
		}
	}
}