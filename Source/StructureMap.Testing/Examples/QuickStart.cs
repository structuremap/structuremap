using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Services.Protocols;
using NUnit.Framework;
using StructureMap.DataAccess;
using StructureMap.DataAccess.Oracle;

namespace StructureMap.Testing.Examples
{
    public class Event
    {
        public long Id { get; set; }
    }

    public interface ICache {}

    public class SimpleCache : ICache{}
    public class SerializationCache : ICache{}

    

    public interface IDataService
    {
        Event LoadEvent(long id);
        void SaveEvent(Event theEvent);
    }

    public class LocalDataService : IDataService
    {
        public Event LoadEvent(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveEvent(Event theEvent)
        {
            throw new System.NotImplementedException();
        }
    }

    public class DataServiceProxy : SoapHttpClientProtocol, IDataService
    {
        public DataServiceProxy(string url)
        {
            this.Url = url;
        }

        public Event LoadEvent(long id)
        {
            throw new System.NotImplementedException();
        }

        public void SaveEvent(Event theEvent)
        {
            throw new System.NotImplementedException();
        }
    }

    public class Repository : IRepository
    {
        private readonly IDataService _service;

        public Repository(IDataService service)
        {
            _service = service;
        }

        public Event Find(long id)
        {
            return null;
        }

        public void Save(Event theEvent)
        {
            
        }
    }

    public interface IRepository{
        Event Find(long id);
        void Save(Event theEvent);
    }




    class SimpleBootstrappingProgram
    {
        private static void Main(string[] args)
        {
            // The code below directs StructureMap to return an OracleDatabaseEngine
            // anytime an IDatabaseEngine is requested
            StructureMapConfiguration.ForRequestedType<IDatabaseEngine>()
                .TheDefaultIsConcreteType<OracleDatabaseEngine>();





        }

        [Test]
        public void StructureMap_should_return_an_OracleDatabaseEngine_for_IDatabaseEngine()
        {
            IDatabaseEngine engine = ObjectFactory.GetInstance<IDatabaseEngine>();

            // For testing the StructureMap configuration, check that the 
            // type returned is really OracleDatabaseEngine
            engine.ShouldBeOfType(typeof(OracleDatabaseEngine));
        }
    }
    


    public class MyApplicationBootstrapper : IBootstrapper
    {
        public void BootstrapStructureMap()
        {
            StructureMapConfiguration.ForRequestedType<IDatabaseEngine>()
                .TheDefaultIsConcreteType<OracleDatabaseEngine>();
        }
    }


    class ModularBootstrappingProgram
    {
        private static void Main(string[] args)
        {
            MyApplicationBootstrapper bootstrapper = new MyApplicationBootstrapper();
            bootstrapper.BootstrapStructureMap();
        }
    }

}
