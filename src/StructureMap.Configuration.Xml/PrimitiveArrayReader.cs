using System;
using System.Xml;
using StructureMap.Pipeline;
using StructureMap.TypeRules;

namespace StructureMap.Configuration.Xml
{
    public class PrimitiveArrayReader : ITypeReader<XmlNode>
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

            throw new NotImplementedException();
            //return new SerializedInstance(array);
        }

        #endregion
    }
}