using System;
using StructureMap.Configuration.Mementos;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    public abstract class Rule
    {
        public Rule()
        {
        }

        public virtual bool IsTrue()
        {
            return true;
        }
    }

    [Pluggable("Complex")]
    public class ComplexRule : Rule
    {
        private bool _Bool;
        private byte _Byte;
        private double _Double;
        private int _Int;
        private long _Long;
        private string _String;
        private string _String2;

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

        public string String
        {
            get { return _String; }
        }


        public string String2
        {
            get { return _String2; }
        }


        public int Int
        {
            get { return _Int; }
        }

        public byte Byte
        {
            get { return _Byte; }
        }

        public long Long
        {
            get { return _Long; }
        }

        public double Double
        {
            get { return _Double; }
        }

        public bool Bool
        {
            get { return _Bool; }
        }

        public static IConfiguredInstance GetInstance()
        {
            ConfiguredInstance memento = new ConfiguredInstance();
            memento.ConcreteKey = "Complex";
            memento.Name = "Sample";

            memento.SetProperty("String", "Red");
            memento.SetProperty("String2", "Green");
            memento.SetProperty("Int", "1");
            memento.SetProperty("Long", "2");
            memento.SetProperty("Byte", "3");
            memento.SetProperty("Double", "4");
            memento.SetProperty("Bool", "true");

            return memento;
        }
    }


    [Pluggable("Color")]
    public class ColorRule : Rule
    {
        private string _Color;
        public string ID = Guid.NewGuid().ToString();

        public ColorRule(string Color)
        {
            _Color = Color;
        }


        public string Color
        {
            get { return _Color; }
        }
    }


    [Pluggable("GreaterThan")]
    public class GreaterThanRule : Rule
    {
        private string _Attribute;
        private int _Value;

        public GreaterThanRule()
        {
        }

        public GreaterThanRule(string Attribute, int Value)
        {
            _Attribute = Attribute;
            _Value = Value;
        }

        public string Attribute
        {
            get { return _Attribute; }
        }

        public int Value
        {
            get { return _Value; }
        }
    }
}