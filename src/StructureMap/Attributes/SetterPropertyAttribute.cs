using System;
using System.Collections;
using System.Reflection;

namespace StructureMap.Attributes
{
    /// <summary>
    /// Marks a Property in a Pluggable class as filled by setter injection 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class SetterPropertyAttribute : Attribute
    {
        #region statics

        public static PropertyInfo[] FindMarkedProperties(Type TPluggedType)
        {
            if (TPluggedType == null)
            {
                return new PropertyInfo[0];
            }

            var list = new ArrayList();

            PropertyInfo[] properties = TPluggedType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                var att =
                    GetCustomAttribute(property, typeof (SetterPropertyAttribute)) as SetterPropertyAttribute;

                if (att != null)
                {
                    list.Add(property);
                }
            }

            return (PropertyInfo[]) list.ToArray(typeof (PropertyInfo));
        }

        #endregion
    }
}