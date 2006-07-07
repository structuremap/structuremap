using System.IO;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
	[TaskName("structuremap.substitute")]
	public class SubstitutionTask : Task
	{
		private string _flag;
		private string _value;
		private string _filePath;

		public SubstitutionTask() : base()
		{
		}

		[TaskAttribute("flag", Required=true)]
		public string Flag
		{
			get { return _flag; }
			set { _flag = value; }
		}

		[TaskAttribute("value", Required=true)]
		public string Value
		{
			get { return _value; }
			set { _value = value; }
		}

		[TaskAttribute("file", Required=true)]
		public string FilePath
		{
			get { return _filePath; }
			set { _filePath = value; }
		}


		protected override void ExecuteTask()
		{
			DoWork();
		}

		public void DoWork()
		{
			string xml = string.Empty;
	
			using (FileStream stream = new FileStream(_filePath, FileMode.Open, FileAccess.Read))
			{
				StreamReader reader = new StreamReader(stream);
				xml = reader.ReadToEnd();
			}
	
			string searchString = string.Format("{{{0}}}", _flag);
			xml = xml.Replace(searchString, _value);
	
			XmlDocument document = new XmlDocument();
			document.LoadXml(xml);
			document.Save(_filePath);
		}
	}
}
