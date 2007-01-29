using System.Threading;
using System.Xml;
using NUnit.Framework;
using StructureMap.Caching;
using StructureMap.Testing.TestData;

namespace StructureMap.Testing.Caching
{
    [TestFixture, Explicit] //These cause NUnit to throw an exception.  They need to be looked at.
    public class FileModificationWatcherTester
    {
        private ManualResetEvent _event;
        private FileModificationWatcher _watcher;

        [SetUp]
        public void SetUp()
        {
            _event = new ManualResetEvent(false);

            DataMother.WriteDocument("Trigger.xml");
            DataMother.WriteDocument("NotTriggered.xml");

            _watcher = new FileModificationWatcher("Trigger.xml");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            DataMother.CleanUp();
        }

        [Test]
        public void ClearedCalledOnCorrectFile()
        {
            MockManagedCache cache = new MockManagedCache(_event, false, true);
            _watcher.AddManagedCache(cache);

            modifyXml("Trigger.XML");
            timeout();
            _event.WaitOne();

            cache.Verify();
        }


        [Test]
        public void ClearedCalledOnInCorrectFile()
        {
            MockManagedCache cache = new MockManagedCache(_event, false, false);
            _watcher.AddManagedCache(cache);

            modifyXml("NotTriggered.XML");
            timeout();
            _event.WaitOne();

            cache.Verify();
        }


        private void modifyXml(string FileName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(FileName);
            doc.DocumentElement.Attributes["State"].Value = "Modified";
            doc.Save(FileName);
        }

        private void timeout()
        {
            Thread thread = new Thread(new ThreadStart(signal));
            thread.Start();
        }

        private void signal()
        {
            Thread.Sleep(500);
            _event.Set();
        }
    }
}