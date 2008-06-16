using System;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class PrimitiveArrayReader : TypeRules, ITypeReader
    {
        #region ITypeReader Members

        public bool CanProcess(Type pluginType)
        {
            return IsPrimitiveArray(pluginType);
        }

        public Instance Read(XmlNode node, Type pluginType)
        {
            Type elementType = pluginType.GetElementType();
            char concatenator = node.GetAttribute("Concatenator", ",").ToCharArray()[0];

            var valueString = node.GetAttribute("Values", string.Empty);
            string[] rawValues = valueString.Split(new[]{concatenator}, StringSplitOptions.RemoveEmptyEntries);


            var array = Array.CreateInstance(elementType, rawValues.Length);
            for (int i = 0; i < rawValues.Length; i++)
            {
                object convertedType = Convert.ChangeType(rawValues[i].Trim(), elementType);
                array.SetValue(convertedType, i);
            }

            return new SerializedInstance(array);
        }

        #endregion
    }
}