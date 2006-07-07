using System;
using System.Xml;
using NAnt.Core;
using NAnt.Core.Attributes;

namespace StructureMap.DeploymentTasks
{
	[TaskName("structuremap.setoverride")]
	public class SetOverrideTask : Task
	{
		private string _configPath;
		private string _profileName = string.Empty;
		private string _typeName = string.Empty;
		private string _defaultKey = string.Empty;

		public SetOverrideTask() : base()
		{

		}

		[TaskAttribute("configPath", Required=true)]
		public string ConfigPath
		{
			get { return _configPath; }
			set { _configPath = value; }
		}

		[TaskAttribute("profileName", Required=false)]
		public string ProfileName
		{
			get { return _profileName; }
			set { _profileName = value; }
		}

		[TaskAttribute("typeName", Required=true)]
		public string TypeName
		{
			get { return _typeName; }
			set { _typeName = value; }
		}

		[TaskAttribute("key", Required=true)]
		public string DefaultKey
		{
			get { return _defaultKey; }
			set { _defaultKey = value; }
		}


		protected override void ExecuteTask()
		{
			XmlDocument document = new XmlDocument();
			document.Load(_configPath);

			ApplyToDocument(document);

			document.Save(_configPath);
		}


		public void ApplyToDocument(XmlDocument document)
		{
			string profileName = determineProfileName(document);

			string message = string.Format("Setting default instance key of type {0} in profile {1} to {2}", _typeName, _defaultKey, profileName);
			Console.WriteLine(message);

			XmlElement profileElement = findProfileElement(profileName, document);
			XmlElement overrideElement = findOverrideElement(profileElement, document);

			overrideElement.SetAttribute("DefaultKey", _defaultKey);
		}

		private string determineProfileName(XmlDocument document)
		{
			string profileName = _profileName;
			if (_profileName == string.Empty)
			{
				profileName = document.DocumentElement.GetAttribute("DefaultProfile");
			}
			return profileName;
		}

		private XmlElement findOverrideElement(XmlElement profileElement, XmlDocument document)
		{
			string overrideXPath = string.Format("Override[@Type='{0}']", _typeName);
			XmlElement overrideElement = (XmlElement) profileElement.SelectSingleNode(overrideXPath);
			if (overrideElement == null)
			{
				overrideElement = document.CreateElement("Override");
				overrideElement.SetAttribute("Type", _typeName);
				profileElement.AppendChild(overrideElement);
			}
			return overrideElement;
		}

		private XmlElement findProfileElement(string profileName, XmlDocument document)
		{
			string profileXPath = string.Format("Profile[@Name='{0}']", profileName);
			XmlElement profileElement = (XmlElement) document.DocumentElement.SelectSingleNode(profileXPath);
			if (profileElement == null)
			{
				profileElement = document.CreateElement("Profile");
				profileElement.SetAttribute("Name", profileName);
				document.DocumentElement.AppendChild(profileElement);
			}
			return profileElement;
		}
	}
}
