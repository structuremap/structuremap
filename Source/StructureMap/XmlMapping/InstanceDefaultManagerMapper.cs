using System.Xml;
using StructureMap.Graph;

namespace StructureMap.XmlMapping
{
	/// <summary>
	/// Reads and Writes InstanceDefaultManager's from and to Xml configuration
	/// </summary>
	public class InstanceDefaultManagerMapper
	{
		private InstanceDefaultManagerMapper()
		{
		}

		public static InstanceDefaultManager ReadFromXml(XmlNode structureMapNode)
		{
			InstanceDefaultManager instanceDefaultManager = new InstanceDefaultManager();

			// Add the Profiles
			foreach (XmlNode nodeProfile in structureMapNode.SelectNodes("Profile"))
			{
				Profile profile = buildProfile(nodeProfile);
				instanceDefaultManager.AddProfile(profile);
			}

			// Add the machine specific overrides
			foreach (XmlNode nodeMachine in structureMapNode.SelectNodes("Machine"))
			{
				MachineOverride machine = buildMachineOverride(nodeMachine, instanceDefaultManager);
				instanceDefaultManager.AddMachineOverride(machine);
			}

			XmlAttribute defaultProfileAttribute = structureMapNode.Attributes["DefaultProfile"];
			if (defaultProfileAttribute != null)
			{
				instanceDefaultManager.DefaultProfileName = defaultProfileAttribute.Value;
			}

			return instanceDefaultManager;
		}


		private static Profile buildProfile(XmlNode nodeProfile)
		{
			string profileName = nodeProfile.Attributes["Name"].Value;
			Profile profile = new Profile(profileName);

			foreach (XmlNode nodeOverride in nodeProfile.SelectNodes("Override"))
			{
				string typeName = nodeOverride.Attributes["Type"].Value;
				string key = nodeOverride.Attributes["DefaultKey"].Value;

				profile.AddOverride(typeName, key);
			}

			return profile;
		}


		private static MachineOverride buildMachineOverride(
			XmlNode nodeMachine,
			InstanceDefaultManager instanceDefaultManager)
		{
			string machineName = nodeMachine.Attributes["Name"].Value;

			MachineOverride returnValue = null;
			if (nodeMachine.Attributes["Profile"] != null)
			{
				string profileName = nodeMachine.Attributes["Profile"].Value;
				Profile profile = instanceDefaultManager.GetProfile(profileName);
				returnValue = new MachineOverride(machineName, profile);
			}
			else
			{
				returnValue = new MachineOverride(machineName);
			}

			foreach (XmlNode nodeOverride in nodeMachine.SelectNodes("Override"))
			{
				string typeName = nodeOverride.Attributes["Type"].Value;
				string key = nodeOverride.Attributes["DefaultKey"].Value;

				returnValue.AddMachineOverride(typeName, key);
			}

			return returnValue;
		}
	}
}