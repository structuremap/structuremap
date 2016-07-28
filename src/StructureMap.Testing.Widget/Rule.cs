using System;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    public abstract class Rule
    {
        public virtual bool IsTrue()
        {
            return true;
        }
    }

    public class ComplexRule : Rule
    {
        [DefaultConstructor]
        public ComplexRule(string String, string String2, int Int, long Long, byte Byte, double Double, bool Bool)
        {
            this.String = String;
            this.String2 = String2;
            this.Int = Int;
            this.Long = Long;
            this.Byte = Byte;
            this.Double = Double;
            this.Bool = Bool;
        }

        /// <summary>
        /// Plugin should find the constructor above, not the "greedy" one below.
        /// </summary>
        /// <param name="String"></param>
        /// <param name="String2"></param>
        /// <param name="Int"></param>
        /// <param name="Long"></param>
        /// <param name="Byte"></param>
        /// <param name="Double"></param>
        /// <param name="Bool"></param>
        /// <param name="extra"></param>
        public ComplexRule(string String, string String2, int Int, long Long, byte Byte, double Double, bool Bool,
                           string extra)
        {
        }

        public string String { get; }


        public string String2 { get; }


        public int Int { get; }

        public byte Byte { get; }

        public long Long { get; }

        public double Double { get; }

        public bool Bool { get; }

        public static IConfiguredInstance GetInstance()
        {
            var memento = new ConfiguredInstance(typeof (ComplexRule));
            memento.Name = "Sample";

            IConfiguredInstance instance = memento;

            instance.Dependencies.Add("String", "Red");
            instance.Dependencies.Add("String2", "Green");
            instance.Dependencies.Add("Int", "1");
            instance.Dependencies.Add("Long", "2");
            instance.Dependencies.Add("Byte", "3");
            instance.Dependencies.Add("Double", "4");
            instance.Dependencies.Add("Bool", "true");

            return memento;
        }
    }


    // SAMPLE: ColorRule
    public class ColorRule : Rule
    {
        public string ID = Guid.NewGuid().ToString();

        public ColorRule(string color)
        {
            Color = color;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public string Color { get; }

        public override string ToString()
        {
            return $"The '{Color}' Rule";
        }
    }
    // ENDSAMPLE


    public class GreaterThanRule : Rule
    {
        public GreaterThanRule()
        {
        }

        public GreaterThanRule(string Attribute, int Value)
        {
            this.Attribute = Attribute;
            this.Value = Value;
        }

        public string Attribute { get; }

        public int Value { get; }
    }
}