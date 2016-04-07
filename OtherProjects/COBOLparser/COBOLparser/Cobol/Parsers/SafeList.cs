using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class SafeList
    {
        public List<string> list = null;
        public int startIndex = 0;

        public string this[int i]
        {
            get
            {
                if (IsOver(i))
                    return String.Empty;
                return list[i];
            }
        }

        public int Count
        {
            get
            {
                return list.Count;
            }
        }

        public bool IsOver(int i)
        {
            return (list == null || list.Count <= i || i < 0);
        }

        public string CurrentString
        {
            get
            {
                return this[startIndex];
            }
        }
    }
}
