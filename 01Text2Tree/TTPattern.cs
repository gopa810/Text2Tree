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
            public int Current = 0;
            public bool Final = false;

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


        public TTTreeNode Parse(TTInputTextFile input)
        {
            TextPosition orig = input.current;
            TextPosition linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            clearFinalizedData(-1, maxIndex);

            while (index <= maxIndex)
            {
                TTPatternEntry pe = Lines[index];
                linePos = input.current;
                if (ParseLine(pe, input))
                {
                    pe.Current++;
                    if (pe.Current >= pe.Min)
                    {
                        if (pe.Current >= pe.Max)
                            pe.Final = true;
                        index++;
                    }
                }
                else
                {
                    // parsing was not successsful
                    // but if we found what we needed, then go to next line
                    // otherwise go back to last not finalized line
                    // and try iterate from there
                    if (pe.Current >= pe.Min && pe.Current <= pe.Max)
                    {
                        // ok, this line is finalized
                        // go to next line
                        pe.Final = true;
                        index++;
                    }
                    else
                    {
                        // find last not finalized line
                        index = findPreviousUnfinishedLine(index);

                        // if found, clear line data after that line
                        // and try to run again parserline for this line
                        // if not found, then we must admit that all parts were finalized
                        // and still we did not satisfied whole pattern
                        // so parsing this pattern is not successful
                        if (index >= 0)
                        {
                            // clear finalized flag and number of found occurences
                            // in all remainig lines
                            clearFinalizedData(index, maxIndex);

                            // restore position in input for parser at line "index"
                            input.current = linePos;
                        }
                        else
                        {
                            // go back to position in input file
                            // as it was before parsing this pattern
                            input.current = orig;
                            return null;
                        }
                    }
                }
            }

            TTTreeNode tn = new TTTreeNode();
            tn.Type = Name;
            tn.Value = ""; /*  value from input file, needs to be extracted according parsed range */

            /* return tn; */
        }

        private void clearFinalizedData(int index, int maxIndex)
        {
            for (int i = index + 1; i <= maxIndex; i++)
            {
                Lines[i].Current = 0;
                Lines[i].Final = false;
            }
        }

        private int findPreviousUnfinishedLine(int index)
        {
            int found = -1;
            for (int i = index - 1; i >= 0; i--)
            {
                if (Lines[i].Final == false)
                {
                    found = i;
                    break;
                }
            }
            return found;
        }

        public bool ParseLine(TTPatternEntry pe, TTInputTextFile input)
        {
            return true;
        }
    }
}
