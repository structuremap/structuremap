using System;

namespace StructureMap.Testing.Widget
{
    public interface IWidget
    {
        void DoSomething();
    }

    [Pluggable("Color", "Only for testing")]
    public class ColorWidget : IWidget, ICloneable
    {
        private string _Color;

        public ColorWidget(string Color)
        {
            _Color = Color;
        }

        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        #region IWidget Members

        public void DoSomething()
        {
            // TODO:  Add ColorWidget.DoSomething implementation
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            ColorWidget colorWidget = obj as ColorWidget;
            if (colorWidget == null) return false;
            return Equals(_Color, colorWidget._Color);
        }

        public override int GetHashCode()
        {
            return _Color != null ? _Color.GetHashCode() : 0;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    [Pluggable("AWidget")]
    public class AWidget : IWidget
    {
        public AWidget()
        {
        }


        public void DoSomething()
        {
            throw new NotImplementedException();
        }
    }

    public class NotPluggableWidget : IWidget
    {
        private readonly string _name;

        public NotPluggableWidget(string name)
        {
            _name = name;
        }


        public string Name
        {
            get { return _name; }
        }

        #region IWidget Members

        public void DoSomething()
        {
            // TODO:  Add NotPluggableWidget.DoSomething implementation
        }

        #endregion
    }

    [Pluggable("Money", "Only for testing")]
    public class MoneyWidget : IWidget
    {
        private double _Amount;


        public double Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        #region IWidget Members

        public void DoSomething()
        {
            // TODO:  Add MoneyWidget.DoSomething implementation
        }

        #endregion
    }


    [Pluggable("Configuration", "Only for testing")]
    public class ConfigurationWidget : IWidget
    {
        private string _String;
        private string _String2;
        private int _Int;
        private long _Long;
        private byte _Byte;
        private double _Double;
        private bool _Bool;

        public ConfigurationWidget(string String, string String2, int Int, long Long, byte Byte, double Double,
                                   bool Bool)
        {
            this.String = String;
            this.String2 = String2;
            this.Int = Int;
            this.Long = Long;
            this.Byte = Byte;
            this.Double = Double;
            this.Bool = Bool;
        }


        public string String
        {
            get { return _String; }
            set { _String = value; }
        }

        public string String2
        {
            get { return _String2; }
            set { _String2 = value; }
        }


        public int Int
        {
            get { return _Int; }
            set { _Int = value; }
        }


        public byte Byte
        {
            get { return _Byte; }
            set { _Byte = value; }
        }


        public long Long
        {
            get { return _Long; }
            set { _Long = value; }
        }


        public double Double
        {
            get { return _Double; }
            set { _Double = value; }
        }


        public bool Bool
        {
            get { return _Bool; }
            set { _Bool = value; }
        }

        [ValidationMethod]
        public void Validate()
        {
            // Throw an exception if Long = 5
            if (Long == 5)
            {
                throw new ApplicationException("Long should not equal 5");
            }
        }

        [ValidationMethod]
        public void Validate2()
        {
        }

        #region IWidget Members

        public void DoSomething()
        {
            // TODO:  Add ConfigurationTarget.DoSomething implementation
        }

        #endregion
    }
}