using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class RedGreenRegistry : Registry
    {
        protected override void configure()
        {
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Red").WithName("Red");
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Green").WithName("Green");
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            RedGreenRegistry redGreenRegistry = obj as RedGreenRegistry;
            if (redGreenRegistry == null) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class YellowBlueRegistry : Registry
    {
        protected override void configure()
        {
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithCtorArg("color").EqualTo("Yellow").WithName("Yellow");
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithProperty("color").EqualTo("Blue").WithName("Blue");
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            YellowBlueRegistry yellowBlueRegistry = obj as YellowBlueRegistry;
            if (yellowBlueRegistry == null) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public class BrownBlackRegistry : Registry
    {
        protected override void configure()
        {
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithProperty("color").EqualTo("Brown").WithName(
                "Brown");
            InstanceOf<IWidget>().Is.OfConcreteType<ColorWidget>().WithProperty("color").EqualTo("Black").WithName(
                "Black");
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            BrownBlackRegistry brownBlackRegistry = obj as BrownBlackRegistry;
            if (brownBlackRegistry == null) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public abstract class PurpleRegistry : Registry
    {
    }
}