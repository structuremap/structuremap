namespace StructureMap.Source
{
    [Pluggable("Templated")]
    public class TemplatedMementoSource : MementoSource
    {
        private readonly MementoSource _innerSource;
        private readonly MementoSource _templateSource;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="innerSource">MementoSource that contains the Memento Templates</param>
        /// <param name="templateSource">MementoSource that contains instances consisting of Template valuee</param>
        public TemplatedMementoSource(MementoSource innerSource, MementoSource templateSource)
        {
            _innerSource = innerSource;
            _templateSource = templateSource;
        }

        public override string Description
        {
            get { return "Templated MementoSource"; }
        }

        protected override InstanceMemento[] fetchInternalMementos()
        {
            InstanceMemento[] rawMementos = _innerSource.GetAllMementos();
            InstanceMemento[] returnValue = new InstanceMemento[rawMementos.Length];

            for (int i = 0; i < returnValue.Length; i++)
            {
                returnValue[i] = resolveMemento(rawMementos[i]);
            }

            return returnValue;
        }

        protected internal override bool containsKey(string instanceKey)
        {
            return _innerSource.containsKey(instanceKey);
        }

        protected override InstanceMemento retrieveMemento(string instanceKey)
        {
            InstanceMemento rawMemento = _innerSource.GetMemento(instanceKey);

            return resolveMemento(rawMemento);
        }

        private InstanceMemento resolveMemento(InstanceMemento rawMemento)
        {
            InstanceMemento returnValue = null;

            string templateName = rawMemento.TemplateName;
            if (templateName == string.Empty)
            {
                returnValue = rawMemento;
            }
            else
            {
                InstanceMemento templateMemento = _templateSource.GetMemento(templateName);

                if (templateMemento == null)
                {
                    throw new StructureMapException(250, templateName);
                }

                returnValue = templateMemento.Substitute(rawMemento);
            }

            return returnValue;
        }

        public override InstanceMemento ResolveMemento(InstanceMemento memento)
        {
            InstanceMemento intermediateMemento = base.ResolveMemento(memento);
            return resolveMemento(intermediateMemento);
        }
    }
}