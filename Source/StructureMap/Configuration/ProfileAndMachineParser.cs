using System.Xml;
using StructureMap.Graph;
using StructureMap.Pipeline;
using StructureMap.Source;

namespace StructureMap.Configuration
{
    public class ProfileAndMachineParser : XmlConstants
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
            _structureMapNode.ForAttributeValue(DEFAULT_PROFILE,
                                                profileName => _profileBuilder.SetDefaultProfileName(profileName));

            forEachNode(PROFILE_NODE).Do(element =>
            {
                string profileName = element.GetAttribute(NAME);
                _profileBuilder.AddProfile(profileName);

                writeOverrides(element,
                               (fullName, defaultKey) =>
                               _profileBuilder.OverrideProfile(new TypePath(fullName), defaultKey), profileName);
            });


            forEachNode(MACHINE_NODE).Do(element =>
            {
                string machineName = element.GetAttribute(NAME);
                string profileName = element.GetAttribute(PROFILE_NODE);

                _profileBuilder.AddMachine(machineName, profileName);

                writeOverrides(element,
                               (fullName, defaultKey) =>
                               _profileBuilder.OverrideMachine(new TypePath(fullName), defaultKey), machineName);
            });
        }


        private void writeOverrides(XmlElement parentElement, WriteOverride function, string profileName)
        {
            parentElement.ForEachChild(OVERRIDE).Do(element => processOverrideElement(function, element, profileName));
        }

        private void processOverrideElement(WriteOverride function, XmlElement overrideElement, string profileName)
        {
            string fullName = overrideElement.GetAttribute(TYPE_ATTRIBUTE);

            overrideElement.IfHasNode(INSTANCE_NODE)
                .Do(element => createOverrideInstance(fullName, element, function, profileName))
                .Else(() =>
                {
                    string defaultKey = overrideElement.GetAttribute(DEFAULT_KEY_ATTRIBUTE);
                    function(fullName, defaultKey);
                });
        }

        private void createOverrideInstance(string fullName, XmlElement instanceElement, WriteOverride function,
                                            string profileName)
        {
            string key = Profile.InstanceKeyForProfile(profileName);
            InstanceMemento memento = _creator.CreateMemento(instanceElement);
            memento.InstanceKey = key;

            var familyPath = new TypePath(fullName);

            _graphBuilder.ConfigureFamily(familyPath, family =>
            {
                family.AddInstance(memento);
                function(fullName, key);
            });
        }

        private XmlExtensions.XmlNodeExpression forEachNode(string xpath)
        {
            return _structureMapNode.ForEachChild(xpath);
        }

        #region Nested type: WriteOverride

        private delegate void WriteOverride(string fullTypeName, string defaultKey);

        #endregion
    }
}