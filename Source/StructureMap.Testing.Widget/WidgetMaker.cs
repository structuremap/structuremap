namespace StructureMap.Testing.Widget
{
    [PluginFamily]
    public abstract class WidgetMaker
    {
        public abstract IWidget MakeWidget();
    }

    [Pluggable("Color")]
    public class ColorWidgetMaker : WidgetMaker
    {
        private string _Color;

        public ColorWidgetMaker(string color)
        {
            _Color = color;
        }

        public string Color
        {
            get { return _Color; }
        }

        public override IWidget MakeWidget()
        {
            return null;
        }
    }

    [Pluggable("Money")]
    public class MoneyWidgetMaker : WidgetMaker
    {
        private double _Amount;


        public double Amount
        {
            get { return _Amount; }
            set { _Amount = value; }
        }

        public override IWidget MakeWidget()
        {
            return null;
        }
    }
}