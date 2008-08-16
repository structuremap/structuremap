using System;
using StructureMap.Pipeline;

namespace StructureMap.Testing.Widget
{
    [PluginFamily, Pluggable("Default")]
    public class GrandChild
    {
        private int _BirthYear;
        private bool _RightHanded;

        public GrandChild(bool RightHanded, int BirthYear)
        {
            _BirthYear = BirthYear;
            _RightHanded = RightHanded;
        }

        public bool RightHanded
        {
            get { return _RightHanded; }
        }

        public int BirthYear
        {
            get { return _BirthYear; }
        }
    }


    [Pluggable("Leftie")]
    public class LeftieGrandChild : GrandChild
    {
        public LeftieGrandChild(int BirthYear) : base(false, BirthYear)
        {
        }
    }


    [PluginFamily, Pluggable("Default")]
    public class Child
    {
        private GrandChild _MyGrandChild;
        private string _Name;

        public Child(string Name, GrandChild MyGrandChild)
        {
            _Name = Name;
            _MyGrandChild = MyGrandChild;
        }

        public string Name
        {
            get { return _Name; }
        }

        public GrandChild MyGrandChild
        {
            get { return _MyGrandChild; }
        }
    }

    [PluginFamily, Pluggable("Default")]
    public class Parent
    {
        private int _Age;
        private string _EyeColor;
        private Child _MyChild;

        public Parent(int Age, string EyeColor, Child MyChild)
        {
            _Age = Age;
            _EyeColor = EyeColor;
            _MyChild = MyChild;
        }

        public int Age
        {
            get { return _Age; }
        }

        public string EyeColor
        {
            get { return _EyeColor; }
        }

        public Child MyChild
        {
            get { return _MyChild; }
        }
    }


    public class ChildLoaderTemplate : InstanceBuilder
    {
        public override Type PluggedType
        {
            get { throw new NotImplementedException(); }
        }


        public override object BuildInstance(IConfiguredInstance instance, BuildSession session)
        {
            var target = new Child(
                instance.GetProperty("Name"),
                (GrandChild) instance.GetChild("MyGrandChild", typeof(GrandChild), session));
            return target;
        }
    }
}