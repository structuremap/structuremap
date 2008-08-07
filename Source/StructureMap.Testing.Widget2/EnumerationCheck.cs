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

    public class SetterBuilder : InstanceBuilder
    {
        public override Type PluggedType
        {
            get { throw new System.NotImplementedException(); }
        }

        public override object BuildInstance(IConfiguredInstance instance, IBuildSession session)
        {
            SetterTarget target = new SetterTarget();


            //instance.ForProperty("Name", s => );
            //target.Name = instance.GetProperty("Name");
            //if (instance.HasProperty("Name")) target.Name = instance.GetProperty("Name");
            //if (instance.HasProperty("Name2")) target.Name = instance.GetProperty("Name2");
            //if (instance.HasProperty("Age")) target.Age = (int)Convert.ChangeType(instance.GetProperty("Age"), typeof(int));
            //if (instance.HasProperty("Breed")) target.Breed = (BreedEnum)Enum.Parse(typeof(BreedEnum), instance.GetProperty("Breed"), true);

            //instance.ForProperty("Name", x => target.Name = x);
            //instance.ForProperty("Age", x => target.Age = (int)Convert.ChangeType(x, typeof (int)));
            //instance.ForProperty("Breed", x => target.Breed = (BreedEnum) Enum.Parse(typeof (BreedEnum), x));

            return target;
        }

        private void setNameProperty(SetterTarget target, string text)
        {
            target.Name = text;
        }
    }

    public class SetterTarget
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public int Age { get; set; }
        public BreedEnum Breed { get; set; }
    }
}