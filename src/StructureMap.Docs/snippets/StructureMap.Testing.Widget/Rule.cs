using System;
using StructureMap.LegacyAttributeSupport;
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

    [Pluggable("Complex")]
    public class ComplexRule : Rule
    {
        private readonly bool _Bool;
        private readonly byte _Byte;
        private readonly double _Double;
        private readonly int _Int;
        private readonly long _Long;
        private readonly string _String;
        private readonly string _String2;

        [DefaultConstructor]
        public ComplexRule(string String, string String2, int Int, long Long, byte Byte, double Double, bool Bool)
        {
            _String = String;
            _String2 = String2;
            _Int = Int;
            _Long = Long;
            _Byte = Byte;
            _Double = Double;
            _Bool = Bool;
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

        public string String { get { return _String; } }


        public string String2 { get { return _String2; } }


        public int Int { get { return _Int; } }

        public byte Byte { get { return _Byte; } }

        public long Long { get { return _Long; } }

        public double Double { get { return _Double; } }

        public bool Bool { get { return _Bool; } }

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


    [Pluggable("Color")]
    // SAMPLE: ColorRule
    public class ColorRule : Rule
    {
        private readonly string _color;
        public string ID = Guid.NewGuid().ToString();

        public ColorRule(string color)
        {
            _color = color;
        }

        public string Name { get; set; }
        public int Age { get; set; }

        public string Color { get { return _color; } }

        public override string ToString()
        {
            return string.Format("The '{0}' Rule", _color);
        }
    }
    // ENDSAMPLE


    [Pluggable("GreaterThan")]
    public class GreaterThanRule : Rule
    {
        private readonly string _Attribute;
        private readonly int _Value;

        public GreaterThanRule()
        {
        }

        public GreaterThanRule(string Attribute, int Value)
        {
            _Attribute = Attribute;
            _Value = Value;
        }

        public string Attribute { get { return _Attribute; } }

        public int Value { get { return _Value; } }
    }
}