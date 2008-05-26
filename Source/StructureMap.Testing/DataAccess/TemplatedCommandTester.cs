using NUnit.Framework;
using StructureMap.DataAccess.Commands;

namespace StructureMap.Testing.DataAccess
{
    [TestFixture]
    public class TemplatedCommandTester
    {
        private void assertSubstitutions(string template, string testMessage, string[] expected)
        {
            TemplatedCommand command = new TemplatedCommand(template, ObjectMother.MSSQLDatabaseEngine());
            string[] actual = command.Substitutions;
            Assert.AreEqual(expected, actual);

            Assert.AreEqual(expected.Length, command.Parameters.Count);
            foreach (string expectedParameter in expected)
            {
                Assert.IsNotNull(command.Parameters[expectedParameter]);
            }
        }

        [Test]
        public void CanResolveOneSubstitution()
        {
            assertSubstitutions("{a}", "simple case", new string[] {"a"});
            assertSubstitutions("{ab}", "simple case", new string[] {"ab"});
            assertSubstitutions("{abc}", "simple case", new string[] {"abc"});
            assertSubstitutions("d{abc}", "simple case", new string[] {"abc"});
            assertSubstitutions("d{abc}e", "simple case", new string[] {"abc"});
            assertSubstitutions("fd{abc}e", "simple case", new string[] {"abc"});
            assertSubstitutions("fd{abc}eg", "simple case", new string[] {"abc"});
        }

        [Test]
        public void CanResolveSubstitutionsWithNoSubstitutions()
        {
            assertSubstitutions("The Message Body", "no substitutions", new string[0]);
        }

        [Test]
        public void CanResolveThreeSubstitutions()
        {
            assertSubstitutions("{a}{b}{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("ggggg{a}{b}{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("{a}hhhhhh{b}{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("{a}{b}jjjjjj{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("{a}{b}{c}lllll", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("asdf{a}{b}fas{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("fda{a}asdf{b}fdas{c}asdf", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("lkjh{a}{b}{c}hjkl", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("asdf{a}{b}asdf{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("{a}asdf{b}{c}asdf", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("fdas{a}aasdf{b}{c}", "three", new string[] {"a", "b", "c"});
            assertSubstitutions("asdf{a}{b}asdf{c}asdf", "three", new string[] {"a", "b", "c"});
        }

        [Test]
        public void CanResolveTwoSubstitutions()
        {
            assertSubstitutions("{abc}{def}", "two", new string[] {"abc", "def"});
            assertSubstitutions("{abc}jjjjj{def}", "two", new string[] {"abc", "def"});
            assertSubstitutions("jjjj{abc}{def}", "two", new string[] {"abc", "def"});
            assertSubstitutions("{abc}{def}uuuuuu", "two", new string[] {"abc", "def"});
            assertSubstitutions("34fg{abc}{def}gjjkk", "two", new string[] {"abc", "def"});
            assertSubstitutions("rrrtrrr{abc}zsdfgsd{def}khgjfghjfgh", "two", new string[] {"abc", "def"});
        }

        [Test]
        public void CreateSql()
        {
            TemplatedCommand command =
                new TemplatedCommand("update {table} set {field} = '{value}'", ObjectMother.MSSQLDatabaseEngine());
            command["table"] = "LOG";
            command["field"] = "UPDATE_DATE";
            command["value"] = "5/1/2005";

            string actual = command.GetSql();
            Assert.AreEqual("update LOG set UPDATE_DATE = '5/1/2005'", actual);
        }

    }
}