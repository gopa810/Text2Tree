using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text2Tree
{
    public class TTTreeNode
    {
        // attributes of tree node
        public string Type;
        public string Value;
        public Dictionary<String, String> Attributes;
        public List<TTTreeNode> Subnodes;
        public TextPosition startPos;
        public TextPosition endPos;

        // constructors
        public TTTreeNode()
        {
            Type = String.Empty;
            Value = String.Empty;
            Attributes = new Dictionary<string,string>();
            Subnodes = new List<TTTreeNode>();
        }

        // methods
        public void SetAttribute(string attrName, string attrVal)
        {
            if (Attributes.ContainsKey(attrName))
                Attributes[attrName] = attrVal;
            Attributes.Add(attrName, attrVal);
        }

        public string GetAttribute(string attrName)
        {
            if (Attributes.ContainsKey(attrName))
                return Attributes[attrName];
            return "";
        }
    }
}
