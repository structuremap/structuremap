using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
    [TaskName("structuremap.substitute")]
    public class SubstitutionTask : Task
    {
        private string _filePath;
        private string _flag;
        private string _value;

        [TaskAttribute("flag", Required = true)]
        public string Flag { get { return _flag; } set { _flag = value; } }

        [TaskAttribute("value", Required = true)]
        public string Value { get { return _value; } set { _value = value; } }

        [TaskAttribute("file", Required = true)]
        public string FilePath { get { return _filePath; } set { _filePath = value; } }


        protected override void ExecuteTask()
        {
            DoWork();
        }

        public void DoWork()
        {
            string xml = string.Empty;

            using (var stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
            {
                var reader = new StreamReader(stream);
                xml = reader.ReadToEnd();
            }

            string searchString = string.Format("{{{0}}}", _flag);
            xml = xml.Replace(searchString, _value);

            var document = new XmlDocument();
            document.LoadXml(xml);
            document.Save(_filePath);
        }
    }
}