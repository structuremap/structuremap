using System;
using StructureMap.Client.TreeNodes;
using StructureMap.Client.Views;

namespace StructureMap.Client.Shell
{
    public class SearchPart
    {
        public static SearchPart[] ParseParts(string searchString)
        {
            searchString = searchString.Replace("http://", "");
            searchString = searchString.Replace("/", "");

            string[] searches = searchString.Split(':');

            SearchPart[] returnValue = new SearchPart[searches.Length];
            for (int i = 0; i < returnValue.Length; i++)
            {
                string[] parts = searches[i].Split('=');
                returnValue[i] = new SearchPart(parts[0], parts[1]);
            }

            return returnValue;
        }

        private readonly string _searchKey;
        private readonly string _searchValue;

        public SearchPart(string searchKey, string searchValue)
        {
            _searchKey = searchKey.ToUpper();
            _searchValue = searchValue;
        }

        public string SearchKey
        {
            get { return _searchKey; }
        }

        public string SearchValue
        {
            get { return _searchValue; }
        }

        public GraphObjectNode FindNode(GraphObjectNode targetNode)
        {
            if (_searchKey == "ID")
            {
                return targetNode.FindById(new Guid(_searchValue));
            }

            if (_searchKey == "PLUGINTYPE")
            {
                GraphObjectNode familiesNode = targetNode.FindChild(ViewConstants.PLUGINFAMILIES);
                return familiesNode.FindChild(_searchValue);
            }

            if (_searchKey == "CONCRETEKEY")
            {
                GraphObjectNode pluginsNode = targetNode.FindChild(ViewConstants.PLUGINS);
                return pluginsNode.FindChild(_searchValue);
            }

            if (_searchKey == "INSTANCEKEY")
            {
                GraphObjectNode instancesNode = targetNode.FindChild(ViewConstants.INSTANCES);
                return instancesNode.FindChild(_searchValue);
            }

            return null;
        }
    }
}