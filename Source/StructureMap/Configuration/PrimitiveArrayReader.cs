using System;
using System.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace StructureMap.Configuration
{
    public class PrimitiveArrayReader : ITypeReader
    {
        #region ITypeReader Members

        public bool CanProcess(Type pluginType)
        {
            return pluginType.IsPrimitiveArray();
        }

        public Instance Read(XmlNode node, Type pluginType)
        {
            Type elementType = pluginType.GetElementType();
            char Delimiter = node.GetAttribute("Delimiter", ",").ToCharArray()[0];

            string valueString = node.GetAttribute("Values", string.Empty);
            string[] rawValues = valueString.Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries);


            Array array = Array.CreateInstance(elementType, rawValues.Length);
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