using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPSpecialRegister: CPBase
    {
        public CPBase Object = null;

        public static CPBase Parse(SafeList list)
        {
            string s = list[list.startIndex];
            CPSpecialRegister sr = null;
            CPBase cp;
            switch (s)
            {
                case "ADDRESS":
                    list.startIndex++;
                    if (list[list.startIndex].Equals("OF"))
                    {
                        list.startIndex++;
                        cp = CPDataName.Parse(list);
                        if (cp == null)
                            RaiseException(list, "Expected data name");
                        sr = new CPSpecialRegister();
                        sr.Value = "ADDRESS OF";
                        sr.Object = cp;
                        return sr;
                    }
                    else
                    {
                        RaiseException(list, "Expected \"OF\"");
                    }
                    break;
                case "DEBUG-ITEM":
                    break;
                case "LENGTH":
                    list.startIndex++;
                    if (list[list.startIndex].Equals("OF"))
                    {
                        list.startIndex++;
                        cp = CPIdentifier.Parse(list);
                        if (cp == null)
                            RaiseException(list, "Expected identifier");
                        sr = new CPSpecialRegister();
                        sr.Value = "LENGTH OF";
                        sr.Object = cp;
                        return sr;
                    }
                    else
                    {
                        RaiseException(list, "Expected \"OF\"");
                    }
                    break;
                case "RETURN-CODE":
                case "SHIFT-OUT":
                case "SHIFT-IN":
                case "SORT-CONTROL":
                case "SORT-CORE-SIZE":
                case "SORT-FILE-SIZE":
                case "SORT-MESSAGE":
                case "SORT-MODE-SIZE":
                case "SORT-RETURN":
                case "TALLY":
                case "WHEN-COMPILED":
                    list.startIndex++;
                    sr = new CPSpecialRegister();
                    sr.Value = s;
                    return sr;
                 
                default:
                    return null;
            }

            return null;
        }

        /*( "ADDRESS" "OF" data-name
| "DEBUG-ITEM"
| "LENGTH" "OF" identifier
| "RETURN-CODE"
| "SHIFT-OUT"
| "SHIFT-IN"
| "SORT-CONTROL"
| "SORT-CORE-SIZE"
| "SORT-FILE-SIZE"
| "SORT-MESSAGE"
| "SORT-MODE-SIZE"
| "SORT-RETURN"
| "TALLY"
| "WHEN-COMPILED" )*/
    }
}
