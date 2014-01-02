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
        /// <summary>
        /// Returns an array of any MethodInfo's on a Type that are marked as ValidationMethod
        /// </summary>
        /// <param name="objectType">CLR Type to search for validation methods</param>
        /// <returns></returns>
        public static MethodInfo[] GetValidationMethods(Type objectType)
        {
            var methodList = new ArrayList();

            var methods = objectType.GetMethods();
            foreach (var method in methods)
            {
                var att =
                    (ValidationMethodAttribute) GetCustomAttribute(method, typeof (ValidationMethodAttribute));

                if (att != null)
                {
                    if (method.GetParameters().Length > 0)
                    {
                        var msg =
                            string.Format(
                                "Method *{0}* in Class *{1}* cannot be a validation method because it has parameters",
                                method.Name, objectType.AssemblyQualifiedName);
                        throw new ApplicationException(msg);
                    }

                    methodList.Add(method);
                }
            }

            var returnValue = new MethodInfo[methodList.Count];
            methodList.CopyTo(returnValue, 0);

            return returnValue;
        }
    }
}