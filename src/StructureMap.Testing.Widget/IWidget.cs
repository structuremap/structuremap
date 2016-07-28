using System;

namespace StructureMap.Testing.Widget
{
    public interface IWidget
    {
        void DoSomething();
    }

    public class ColorWidget : IWidget, ICloneable
    {
        private readonly string _Color;

        public ColorWidget(string color)
        {
            _Color = color;
        }

        public string Color { get { return _Color; } }

        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        #region IWidget Members

        public void DoSomething()
        {
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var colorWidget = obj as ColorWidget;
            if (colorWidget == null) return false;
            return Equals(_Color, colorWidget._Color);
        }

        public override int GetHashCode()
        {
            return _Color != null ? _Color.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return string.Format("Color: {0}", Color);
        }
    }

    public class AWidget : IWidget, ICloneable
    {
        #region ICloneable Members

        public object Clone()
        {
            return MemberwiseClone();
        }

        #endregion

        #region IWidget Members

        public void DoSomething()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class NotPluggableWidget : IWidget
    {
        private readonly string _name;

        public NotPluggableWidget(string name)
        {
            _name = name;
        }


        public string Name { get { return _name; } }

        #region IWidget Members

        public void DoSomething()
        {
        }

        #endregion
    }

    public class MoneyWidget : IWidget
    {
        public double Amount { get; set; }

        #region IWidget Members

        public void DoSomething()
        {
        }

        #endregion
    }


    public class ConfigurationWidget : IWidget
    {
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


        public string String { get; set; }

        public string String2 { get; set; }


        public int Int { get; set; }


        public byte Byte { get; set; }


        public long Long { get; set; }


        public double Double { get; set; }


        public bool Bool { get; set; }

        #region IWidget Members

        public void DoSomething()
        {
        }

        #endregion

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
    }
}