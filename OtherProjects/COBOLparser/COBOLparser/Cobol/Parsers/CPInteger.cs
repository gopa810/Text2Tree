using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPInteger: CPBase
    {
        public static CPBase Parse(SafeList list)
        {
            int i = 0;
            if (int.TryParse(list.CurrentString, out i))
            {
                CPInteger cpi = new CPInteger();
                cpi.Value = list.CurrentString;
                list.startIndex++;
                return cpi;
            }

            return null;
        }
    }
}
