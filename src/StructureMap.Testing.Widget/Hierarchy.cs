namespace StructureMap.Testing.Widget
{
    [Pluggable("Default")]
    public class GrandChild
    {
        private readonly int _BirthYear;
        private readonly bool _RightHanded;

        public GrandChild(bool RightHanded, int BirthYear)
        {
            _BirthYear = BirthYear;
            _RightHanded = RightHanded;
        }

        public bool RightHanded { get { return _RightHanded; } }

        public int BirthYear { get { return _BirthYear; } }
    }


    [Pluggable("Leftie")]
    public class LeftieGrandChild : GrandChild
    {
        public LeftieGrandChild(int BirthYear)
            : base(false, BirthYear)
        {
        }
    }


    [Pluggable("Default")]
    public class Child
    {
        private readonly GrandChild _MyGrandChild;
        private readonly string _Name;

        public Child(string Name, GrandChild MyGrandChild)
        {
            _Name = Name;
            _MyGrandChild = MyGrandChild;
        }

        public string Name { get { return _Name; } }

        public GrandChild MyGrandChild { get { return _MyGrandChild; } }
    }

    [Pluggable("Default")]
    public class Parent
    {
        private readonly int _Age;
        private readonly string _EyeColor;
        private readonly Child _MyChild;

        public Parent(int Age, string EyeColor, Child MyChild)
        {
            _Age = Age;
            _EyeColor = EyeColor;
            _MyChild = MyChild;
        }

        public int Age { get { return _Age; } }

        public string EyeColor { get { return _EyeColor; } }

        public Child MyChild { get { return _MyChild; } }
    }
}