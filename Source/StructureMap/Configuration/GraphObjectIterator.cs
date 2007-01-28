namespace StructureMap.Configuration
{
    public class GraphObjectIterator
    {
        private readonly IConfigurationVisitor _visitor;

        public GraphObjectIterator(IConfigurationVisitor visitor)
        {
            _visitor = visitor;
        }

        public void Visit(GraphObject startNode)
        {
            _visitor.StartObject(startNode);
            startNode.AcceptVisitor(_visitor);

            foreach (GraphObject child in startNode.Children)
            {
                Visit(child);
            }

            _visitor.EndObject(startNode);
        }
    }
}