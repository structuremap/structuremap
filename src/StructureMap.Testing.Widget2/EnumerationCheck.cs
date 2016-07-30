
namespace StructureMap.Testing.Widget2
{
    public enum BreedEnum
    {
        Hereford,
        Angus,
        Longhorn, 
        Beefmaster
    }

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


    public class SetterTarget
    {
        public string Name { get; set; }
        public string Name2 { get; set; }
        public int Age { get; set; }
        public BreedEnum Breed { get; set; }
    }
}