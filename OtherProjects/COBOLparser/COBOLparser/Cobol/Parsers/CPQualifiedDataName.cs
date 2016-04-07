using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COBOLparser.Cobol.Parsers
{
    public class CPQualifiedDataName: CPBase
    {
        public class QDNE
        {
            public enum QDNET {
                None, InData, OfData, InFile, OfFile
            };

            QDNET type;
            string dataName;

            public QDNE(QDNE.QDNET typ, string name)
            {
                type = typ;
                dataName = name;
            }
        }

        public List<QDNE> parts = new List<QDNE>();

        public static override CPBase Parse(SafeList list)
        {
            CPBase cp, cp2 = null;

            cp = CPSpecialRegister.Parse(list);
            if (cp != null)
                return cp;

            cp = CPDataName.Parse(list);

            if (cp == null)
                return null;

            CPQualifiedDataName qdn = new CPQualifiedDataName();
            qdn.parts.Add(new QDNE(QDNE.QDNET.None, cp.Value));

            do
            {
                cp = StringToken.Parse(list, "IN", "OF");
                cp2 = null;
                if (cp != null)
                {
                    cp2 = CPDataName.Parse(list);
                    if (cp2 == null)
                    {
                        cp2 = CPFileName.Parse(list);
                        if (cp2 != null)
                        {
                            qdn.parts.Add(new QDNE(cp.Value.Equals("IN") ? QDNE.QDNET.InFile : QDNE.QDNET.OfFile, cp2.Value));
                            break;
                        }
                        else
                        {
                            RaiseException(list, "Expected dataname or filename");
                        }
                    }
                    else
                    {
                        qdn.parts.Add(new QDNE(cp.Value.Equals("IN") ? QDNE.QDNET.InData : QDNE.QDNET.OfData, cp2.Value));
                    }
                }
            }
            while (cp2 != null);

            return qdn;
        }
    }
}
