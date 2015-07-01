using NUnit.Framework;
using Shouldly;
using StructureMap.Building;

namespace StructureMap.Testing.Building
{
    [TestFixture]
    public class complex_concrete_build_with_child_dependencies
    {
        private Game game;

        [SetUp]
        public void SetUp()
        {
            var build = new ConcreteBuild<Game>();
            build.Constructor.Add(new ConcreteBuild<SportsTeam>()
                .ConstructorArgs("San Diego", "Chargers"));

            build.Constructor.Add(new ConcreteBuild<SportsTeam>()
                .ConstructorArgs("Kansas City", "Chiefs"));

            build.Set(x => x.Stadium, "Qualcomm Stadium");

            build.Set(x => x.Referee, new ConcreteBuild<Referee>().ConstructorArgs("John", "Smith"));

            var session = new FakeBuildSession();
            game = build.Build(session, session).As<Game>();
        }

        [Test]
        public void with_child_constructors()
        {
            game.Home.City.ShouldBe("San Diego");
            game.Home.Mascot.ShouldBe("Chargers");
            game.Visitor.City.ShouldBe("Kansas City");
            game.Visitor.Mascot.ShouldBe("Chiefs");
        }

        [Test]
        public void with_child_setter_value()
        {
            game.Stadium.ShouldBe("Qualcomm Stadium");
        }

        [Test]
        public void with_child_setter_built_by_constructor()
        {
            game.Referee.ShouldBe(new Referee("John", "Smith"));
        }
    }

    public class Game
    {
        private readonly SportsTeam _home;
        private readonly SportsTeam _visitor;

        public Game(SportsTeam home, SportsTeam visitor)
        {
            _home = home;
            _visitor = visitor;
        }

        public SportsTeam Home
        {
            get { return _home; }
        }

        public SportsTeam Visitor
        {
            get { return _visitor; }
        }

        public Referee Referee { get; set; }
        public string Stadium { get; set; }
    }

    public class Referee
    {
        private readonly string _first;
        private readonly string _last;

        public Referee(string first, string last)
        {
            _first = first;
            _last = last;
        }

        protected bool Equals(Referee other)
        {
            return string.Equals(_first, other._first) && string.Equals(_last, other._last);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Referee) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_first != null ? _first.GetHashCode() : 0)*397) ^ (_last != null ? _last.GetHashCode() : 0);
            }
        }
    }

    public class SportsTeam
    {
        private readonly string _city;
        private readonly string _mascot;

        public SportsTeam(string city, string mascot)
        {
            _city = city;
            _mascot = mascot;
        }

        public string City
        {
            get { return _city; }
        }

        public string Mascot
        {
            get { return _mascot; }
        }
    }
}