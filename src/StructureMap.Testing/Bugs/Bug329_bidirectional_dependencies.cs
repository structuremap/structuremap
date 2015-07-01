using System;
using NUnit.Framework;
using Shouldly;
using StructureMap.Building;

namespace StructureMap.Testing.Bugs
{
    [TestFixture]
    public class Bug329_bidirectional_dependencies
    {
        [Test]
        public void Singleton_WhenFailsToConstruct_ShouldNotThrowBiDiErrorsOnFurtherAttemptsToConstruct()
        {
            var container = new Container(_ =>
                _.ForSingletonOf<SomeSingletonThatConnectsToDatabaseOnConstruction>()
                    .Use<SomeSingletonThatConnectsToDatabaseOnConstruction>());

            // 1st attempt : database is down, expect "Database is down" exception
            DatabaseStatus.DatabaseIsDown = true;

            Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                container.GetInstance<SomeSingletonThatConnectsToDatabaseOnConstruction>();
            }).InnerException.ShouldBeOfType<DivideByZeroException>().Message.ShouldContain("Database is down");

            // 2nd attempt : database is still down, still expect "Database is down" exception
            Exception<StructureMapBuildException>.ShouldBeThrownBy(() =>
            {
                container.GetInstance<SomeSingletonThatConnectsToDatabaseOnConstruction>();
            }).InnerException.ShouldBeOfType<DivideByZeroException>().Message.ShouldContain("Database is down");

            

            // The database has now come online.
            DatabaseStatus.DatabaseIsDown = false;

            // If the database is up, the construction of the instance should now succeed, but
            // instead still throws a bi-di error
            var attempt3 = container.GetInstance<SomeSingletonThatConnectsToDatabaseOnConstruction>();
            attempt3.ShouldNotBeNull();

        }


    }

    public static class DatabaseStatus
    {
        public static bool DatabaseIsDown { get; set; }
    }

    public class SomeSingletonThatConnectsToDatabaseOnConstruction
    {
        public SomeSingletonThatConnectsToDatabaseOnConstruction()
        {
            if (DatabaseStatus.DatabaseIsDown)
                throw new DivideByZeroException("Database is down");
        }
    }
}