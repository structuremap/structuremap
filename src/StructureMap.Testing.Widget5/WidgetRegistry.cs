using StructureMap.Configuration.DSL;
using StructureMap.Testing.Widget;

namespace StructureMap.Testing.Widget5
{
    public class RedGreenRegistry : Registry
    {
        public RedGreenRegistry()
        {
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Red").Named("Red");
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Green").Named(
                "Green");
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var redGreenRegistry = obj as RedGreenRegistry;
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
        public YellowBlueRegistry()
        {
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Yellow").Named("Yellow");
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Blue").Named("Blue");
        }

        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var yellowBlueRegistry = obj as YellowBlueRegistry;
            if (yellowBlueRegistry == null) return false;
            return true;
        }

        public override int GetHashCode()
        {
            return 0;
        }
    }

    public interface ITypeThatHasAttributeButIsNotInRegistry
    {
    }

    public interface IInterfaceInWidget5
    {
    }

    [Pluggable("TheType")]
    public class TypeThatHasAttributeButIsNotInRegistry : ITypeThatHasAttributeButIsNotInRegistry
    {
    }

    public class BrownBlackRegistry : Registry
    {
        public BrownBlackRegistry()
        {
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Brown").Named("Brown");
            For<IWidget>().Add<ColorWidget>().Ctor<string>("color").Is("Black").Named("Black");
        }


        public override bool Equals(object obj)
        {
            if (this == obj) return true;
            var brownBlackRegistry = obj as BrownBlackRegistry;
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