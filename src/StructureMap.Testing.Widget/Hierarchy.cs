
namespace StructureMap.Testing.Widget
{
    public class GrandChild
    {
        public GrandChild(bool RightHanded, int BirthYear)
        {
            this.BirthYear = BirthYear;
            this.RightHanded = RightHanded;
        }

        public bool RightHanded { get; }

        public int BirthYear { get; }
    }


    public class LeftieGrandChild : GrandChild
    {
        public LeftieGrandChild(int BirthYear)
            : base(false, BirthYear)
        {
        }
    }


    public class Child
    {
        public Child(string Name, GrandChild MyGrandChild)
        {
            this.Name = Name;
            this.MyGrandChild = MyGrandChild;
        }

        public string Name { get; }

        public GrandChild MyGrandChild { get; }
    }

    public class Parent
    {
        public Parent(int Age, string EyeColor, Child MyChild)
        {
            this.Age = Age;
            this.EyeColor = EyeColor;
            this.MyChild = MyChild;
        }

        public int Age { get; }

        public string EyeColor { get; }

        public Child MyChild { get; }
    }
}