using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol
{
    public class PathElement
    {
        public static char PathSeparatorChar = '/';
        public static char ElementSeparatorChar = '.';
        public string Type = string.Empty;
        public string Value = string.Empty;

        public PathElement()
        {
        }

        public PathElement(string s)
        {
            string[] p = s.Split(ElementSeparatorChar);
            if (p.Length == 1)
            {
                Type = p[0];
                Value = string.Empty;
            }
            else if (p.Length >= 2)
            {
                Type = p[0];
                Value = p[1];
            }
        }

        public PathElement(string t, string v)
        {
            Type = t;
            Value = v;
        }

        public string NodeString
        {
            get
            {
                return string.Format("{0}{1}{2}{3}", PathElement.PathSeparatorChar, Type, PathElement.ElementSeparatorChar, Value);
            }
        }
    }
}
