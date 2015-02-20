using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextTreeParser
{
    public class TTAtom
    {
        public TTAtom next = null;

        public string Type;
        public string Value;
        private Dictionary<String, String> Attributes;
        public TextPosition startPos;
        public TextPosition endPos;

        public TTAtom last
        {
            get
            {
                TTAtom t = this;
                while (t.next != null)
                {
                    t = t.next;
                }
                return t;
            }
        }

        // methods
        public void SetAttribute(string attrName, string attrVal)
        {
            if (Attributes == null)
                Attributes = new Dictionary<string, string>();
            if (Attributes.ContainsKey(attrName))
                Attributes[attrName] = attrVal;
            Attributes.Add(attrName, attrVal);
        }

        public string GetAttribute(string attrName)
        {
            if (Attributes == null)
                return "";
            if (Attributes.ContainsKey(attrName))
                return Attributes[attrName];
            return "";
        }
    }
}
