using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPAlphabeticUserDefinedWord: CPBase
    {
        public CPAlphabeticUserDefinedWord(string s)
        {
            Value = s;
        }

        public static override CPBase Parse(SafeList list)
        {
            string s = list[list.startIndex];

            if (s.Length > 0 && Char.IsLetterOrDigit(s[0]))
            {
                foreach (char c in s)
                {
                    if (!Char.IsLetterOrDigit(c) && c != '-')
                        return null;
                }
            }
            else
            {
                return null;
            }

            list.startIndex++;
            return new CPAlphabeticUserDefinedWord(s);
        }
    }
}
