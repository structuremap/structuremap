using System;

namespace StructureMap.Testing.Widget2
{
    public enum BreedEnum
    {
        Hereford,
        Angus,
        Longhorn
    }

    [Pluggable("Cow")]
    public class Cow
    {
        public BreedEnum Breed;
        public long Weight;
        public string Name;

        public Cow(long Weight, BreedEnum Breed, string Name)
        {
            this.Breed = Breed;
            this.Weight = Weight;
            this.Name = Name;
        }
    }


    public class CowBuilder : InstanceBuilder
    {
        public override string ConcreteTypeKey
        {
            get { return "Default"; }
        }

        public override string PluggedType
        {
            get { return null; }
        }


        public override string PluginType
        {
            get { return null; }
        }


        public override object BuildInstance(InstanceMemento memento)
        {
            return new Cow(
                long.Parse(memento.GetProperty("Weight")),
                (BreedEnum) Enum.Parse(typeof (BreedEnum), memento.GetProperty("Breed"), true),
                memento.GetProperty("Name"));
        }
    }
}