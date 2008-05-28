using System;
using StructureMap.Pipeline;

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
        public string Name;
        public long Weight;

        public Cow(long Weight, BreedEnum Breed, string Name)
        {
            this.Breed = Breed;
            this.Weight = Weight;
            this.Name = Name;
        }
    }


    public class CowBuilder : InstanceBuilder
    {
        public Type ThePluginType
        {
            get
            {
                return typeof (Cow);
            }
        }

        public override Type PluggedType
        {
            get { return typeof (Cow); }
        }


        public override object BuildInstance(IConfiguredInstance instance, StructureMap.Pipeline.IBuildSession session)
        {
            return new Cow(
                long.Parse(instance.GetProperty("Weight")),
                (BreedEnum) Enum.Parse(typeof (BreedEnum), instance.GetProperty("Breed"), true),
                instance.GetProperty("Name"));
        }
    }
}