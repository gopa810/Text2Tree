using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TrepInterpreter
{
    public class SVNumber: SValue
    {
        public static void AssertIsNumber(SValue sv)
        {
            if (!(sv is SVNumber))
                throw new Exception("Expected number at " + sv.nodeId);
        }
    }
}
