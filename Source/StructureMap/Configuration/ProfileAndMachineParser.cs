using System.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    // TODO:  3.5 cleanup
    public class ProfileAndMachineParser
    {
        private readonly XmlMementoCreator _creator;
        private readonly IGraphBuilder _graphBuilder;
        private readonly IProfileBuilder _profileBuilder;
        private readonly XmlNode _structureMapNode;

        public ProfileAndMachineParser(IGraphBuilder graphBuilder, XmlNode structureMapNode, XmlMementoCreator creator)
        {
            _profileBuilder = graphBuilder.GetProfileBuilder();
            _graphBuilder = graphBuilder;
            _structureMapNode = structureMapNode;
            _creator = creator;
        }

        public void Parse()
        {
            // TODO:  3.5 cleanup
            XmlNode defaultProfileNode = _structureMapNode.Attributes.GetNamedItem(XmlConstants.DEFAULT_PROFILE);
            if (defaultProfileNode != null)
            {
                _profileBuilder.SetDefaultProfileName(defaultProfileNode.InnerText);
            }

            foreach (XmlElement profileElement in findNodes(XmlConstants.PROFILE_NODE))
            {
                string profileName = profileElement.GetAttribute(XmlConstants.NAME);
                _profileBuilder.AddProfile(profileName);

                writeOverrides(profileElement,
                               delegate(string fullName, string defaultKey) { _profileBuilder.OverrideProfile(new TypePath(fullName), defaultKey); }, profileName);
            }

            foreach (XmlElement machineElement in findNodes(XmlConstants.MACHINE_NODE))
            {
                string machineName = machineElement.GetAttribute(XmlConstants.NAME);
                string profileName = machineElement.GetAttribute(XmlConstants.PROFILE_NODE);

                _profileBuilder.AddMachine(machineName, profileName);

                writeOverrides(machineElement,
                               delegate(string fullName, string defaultKey) { _profileBuilder.OverrideMachine(new TypePath(fullName), defaultKey); }, machineName);
            }
        }


        private void writeOverrides(XmlElement parentElement, WriteOverride function, string profileName)
        {
            foreach (XmlElement overrideElement in parentElement.SelectNodes(XmlConstants.OVERRIDE))
            {
                processOverrideElement(function, overrideElement, profileName);
            }
        }

        private void processOverrideElement(WriteOverride function, XmlElement overrideElement, string profileName)
        {
            string fullName = overrideElement.GetAttribute(XmlConstants.TYPE_ATTRIBUTE);

            XmlElement instanceElement = (XmlElement) overrideElement.SelectSingleNode(XmlConstants.INSTANCE_NODE);
            if (instanceElement == null)
            {
                string defaultKey = overrideElement.GetAttribute(XmlConstants.DEFAULT_KEY_ATTRIBUTE);
                function(fullName, defaultKey);
            }
            else
            {
                createOverrideInstance(fullName, instanceElement, function, profileName);
            }
        }

        private void createOverrideInstance(string fullName, XmlElement instanceElement, WriteOverride function,
                                            string profileName)
        {
            string key = Profile.InstanceKeyForProfile(profileName);
            InstanceMemento memento = _creator.CreateMemento(instanceElement);
            memento.InstanceKey = key;

            TypePath familyPath = new TypePath(fullName);

            _graphBuilder.ConfigureFamily(familyPath, delegate(PluginFamily family)
                                                          {
                                                              family.AddInstance(memento);
                                                              function(fullName, key);
                                                          });
        }

        private XmlNodeList findNodes(string nodeName)
        {
            return _structureMapNode.SelectNodes(nodeName);
        }

        #region Nested type: WriteOverride

        private delegate void WriteOverride(string fullTypeName, string defaultKey);

        #endregion
    }
}