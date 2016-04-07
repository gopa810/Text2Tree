using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using COBOLparser.Cobol;

namespace COBOLparser.Cobol.Parsers
{
    public class CPBase
    {
        public string Value;

        public virtual CPBase Parse(SafeList list)
        {
            throw new NotImplementedException();
        }

        public virtual CPBase Parse(SafeList list, params string[] data)
        {
            throw new NotImplementedException();
        }

        public static void RaiseException(SafeList stats, string message)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Problem: {0}\n", message);
            int i = stats.startIndex;
            int length = 0;
            foreach (string s in stats.list)
            {
                if (i > 0)
                {
                    length += s.Length + 1;
                }
                else if (i == 0)
                {
                    sb.AppendFormat(" <*> ");
                }
                i--;
                sb.AppendFormat("{0} ", s);
            }
            sb.AppendLine();
            sb.Append("^".PadLeft(length));
            sb.AppendLine();
            throw new Exception(sb.ToString());
        }
    }
}
