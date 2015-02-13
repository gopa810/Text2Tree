using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Text2Tree
{
    public class TTPattern
    {
        protected class TTPatternEntry
        {
            public int Min = 0;
            public int Max = Int32.MaxValue;

            /// <summary>
            /// This may be char, TTCharset or TTPattern
            /// </summary>
            public object EntryObject = null;
        }

        public string Name = string.Empty;
        protected List<TTPatternEntry> Lines = new List<TTPatternEntry>();

        public TTPattern()
        {
        }

        public TTPattern(string str)
        {
            foreach (char c in str)
            {
                addChar(1, 1, c);
            }
        }

        public void addChar(int min, int max, char c)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = c;

            Lines.Add(pe);
        }

        public void addChars(int min, int max, string str)
        {
            TTCharset cs = new TTCharset();
            cs.addChars(str);

            addCharset(min, max, cs);
        }

        public void addCharset(int min, int max, TTCharset cs)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = cs;
            Lines.Add(pe);
        }

        public void addPattern(int min, int max, TTPattern pat)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = pat;
            Lines.Add(pe);
        }
        
    }
}
