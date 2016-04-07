using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPIdentifier: CPBase
    {
        public CPQualifiedDataName data = null;
        public List<CPBase> accs = null;
        public CPBase scopeStart = null;
        public CPBase scopeLength = null;

        public static override CPBase Parse(SafeList list)
        {
            CPBase cp, cp2;
            int backi;
            cp = CPQualifiedDataName.Parse(list);

            if (cp == null)
                RaiseException(list, "Expected qualified-data-name");

            CPIdentifier cpi = new CPIdentifier();
            cpi.data = cp as CPQualifiedDataName;

            do
            {
                backi = list.startIndex;
                cp = StringToken.Parse(list, "(");
                if (cp != null)
                {
                    cp = CPSubscript.Parse(list);
                    if (cp != null)
                    {
                        cp2 = StringToken.Parse(list, ")");
                        if (cp2 != null)
                        {
                            if (cpi.accs == null)
                                cpi.accs = new List<CPBase>();
                            cpi.accs.Add(cp);
                        }
                        else
                        {
                            list.startIndex = backi;
                            break;
                        }
                    }
                    else
                    {
                        list.startIndex = backi;
                        break;
                    }
                }
                else
                {
                    list.startIndex = backi;
                    break;
                }
            }
            while (true);

            backi = list.startIndex;
            cp = StringToken.Parse(list, "(");
            if (cp != null)
            {
                cp = CPLeftmostCharacterPosition.Parse(list);
                if (cp != null)
                {
                    cpi.scopeStart = cp;
                    cp = StringToken.Parse(list, ":");
                    if (cp != null)
                    {
                        cp = CPLength.Parse(list);
                        if (cp != null)
                        {
                            cpi.scopeLength = cp;
                        }

                        cp = StringToken.Parse(list, ")");
                        if (cp == null)
                            RaiseException(list, "Expected )");
                    }
                    else
                    {
                        RaiseException(list, "Expected :");
                    }
                }
                else
                {
                    list.startIndex = backi;
                }
            }
            else
            {
                list.startIndex = backi;
            }

            return cpi;
        }
        /*
                     int i = startIndex;
            int pp;
            CPBase idd;
            List<CPBase> listdd;

            idd = TryQualifiedDataName(stats, ref i, null);
            if (idd == null)
            {
                RaiseException(stats, i, "Expected qualified-data-name");
            }

            do
            {
                listdd = TryZSerial(stats, ref i, new IPE(TryZ, "("), new IPE(TrySubscript), new IPE(TryZ, ")"));
            }
            while(listdd != null);

            listdd = TryZSerial(stats, ref i, new IPE(TryZ, "("), new IPE(TryLeftmostCharacterPosition),
                new IPE(TryZ, ":"), new IPE(TryLength, null, 0, 1), new IPE(TryZ, ")"));

            startIndex = i;
            return idd;
         */
    }
}
