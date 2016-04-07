using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class StringToken: CPBase
    {
        public StringToken(string s)
        {
            Value = s;
        }

        public static override CPBase Parse(SafeList list, params string[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (list[list.startIndex].Equals(data[i]))
                {
                    list.startIndex++;
                    return new StringToken(data[i]);
                }
            }

            return null;
        }

    }
}
