﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPDataName: CPBase
    {
        public static CPBase Parse(SafeList list)
        {
            return CPAlphabeticUserDefinedWord.Parse(list);
        }
    }
}
