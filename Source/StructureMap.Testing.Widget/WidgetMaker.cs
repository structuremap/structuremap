namespace StructureMap.Testing.Widget
{
    [PluginFamily]
    public abstract class WidgetMaker
    {
        public abstract IWidget MakeWidget();
    }

    [Pluggable("Color", "Only for testing")]
    public class ColorWidgetMaker : WidgetMaker
    {
        private string _Color;

        public ColorWidgetMaker(string Color)
        {
            _Color = Color;
        }

        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }

        public override IWidget MakeWidget()
        {
            return null;
        }
    }

    [Pluggable("Money", "Only for testing")]
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