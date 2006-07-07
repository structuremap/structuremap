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

		public static PropertyInfo[] FindMarkedProperties(Type pluggedType)
		{
			if (pluggedType == null)
			{
				return new PropertyInfo[0];
			}

			ArrayList list = new ArrayList();

			PropertyInfo[] properties = pluggedType.GetProperties();
			foreach (PropertyInfo property in properties)
			{
				SetterPropertyAttribute att =
					Attribute.GetCustomAttribute(property, typeof (SetterPropertyAttribute)) as SetterPropertyAttribute;

				if (att != null)
				{
					list.Add(property);
				}
			}

			return (PropertyInfo[]) list.ToArray(typeof (PropertyInfo));
		}

		#endregion

		public SetterPropertyAttribute()
		{
		}
	}
}