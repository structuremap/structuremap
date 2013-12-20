using StructureMap.LegacyAttributeSupport;

namespace StructureMap.Testing.Widget
{
    public abstract class WidgetMaker
    {
        public abstract IWidget MakeWidget();
    }

    [Pluggable("Color")]
    public class ColorWidgetMaker : WidgetMaker
    {
        private readonly string _Color;

        public ColorWidgetMaker(string color)
        {
            _Color = color;
        }

        public string Color { get { return _Color; } }

        public override IWidget MakeWidget()
        {
            return null;
        }
    }

    [Pluggable("Money")]
    public class MoneyWidgetMaker : WidgetMaker
    {
        public double Amount { get; set; }

        public override IWidget MakeWidget()
        {
            return null;
        }
    }
}