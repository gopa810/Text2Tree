using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TextTreeParser
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

            public override string ToString()
            {
                return string.Format("{0}:{1}:{2}", Min, Current, Max);
            }
        }

        public string Name = string.Empty;
        protected List<TTPatternEntry> Lines = new List<TTPatternEntry>();
        public bool patternParallel = false;

        public TTPattern()
        {
        }

        public TTPattern(string str)
        {
            Name = str;
        }

        public bool IsParallel
        {
            get { return patternParallel; }
            set { patternParallel = value; }
        }

        public void addString(int min, int max, string str)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = str;

            Lines.Add(pe);
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
            TTCharset cs = new TTCharset("_charset_" + str);
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


        public TTAtom ParseAtom(TTInputTextFile input)
        {
            TextPosition orig = input.next;
            bool b;

            if (patternParallel)
            {
                b = ParseParallel(input, orig);
            }
            else
            {
                b = ParseSerial(input, orig);
            }

            if (b)
            {
                TTAtom tn = new TTAtom();
                tn.Type = Name;
                tn.startPos = orig;
                tn.endPos = input.next;
                tn.Value = input.stringFromRange(orig.position, input.next.position - 1); /*  value from input file, needs to be extracted according parsed range */
                return tn;
            }

            return null;
        }

        public TTTreeNode ParseTree(TTAtomList input)
        {
            TTAtom orig = input.current;
            bool b;

            if (patternParallel)
            {
                b = ParseParallel(input, orig);
            }
            else
            {
                b = ParseSerial(input, orig);
            }

            if (b)
            {
                TTTreeNode tn = new TTTreeNode();
                tn.Name = Name;
                TTAtom iter = orig;
                while (iter != input.current)
                {
                    TTTreeNode ta = new TTTreeNode();
                    tn.atom = iter;
                    iter = iter.next;
                    tn.addSubnode(ta);
                }
                return tn;
            }

            return null;
        }
        private bool ParseParallel(TTInputTextFile input, TextPosition orig)
        {
            TextPosition maxPos = orig;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.next = orig;
                if (ParseLine(pe, input))
                {
                    // this is like negation
                    // if this line was hit, then stop using this parser
                    if (pe.Max < 0)
                    {
                        input.next = orig;
                        return false;
                    }
                    if (maxPos.position < input.next.position)
                    {
                        maxPos = input.next;
                        maxLine = pe;
                    }
                }
            }

            if (maxLine != null && maxPos.position > orig.position)
            {
                input.next = maxPos;
                return true;
            }

            return false;
        }

        private bool ParseParallel(TTAtomList input, TTAtom orig)
        {
            TTAtom maxPos = orig;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.current = orig;
                if (ParseLine(pe, input))
                {
                    // this is like negation
                    // if this line was hit, then stop using this parser
                    if (pe.Max < 0)
                    {
                        input.current = orig;
                        return false;
                    }
                    if (maxPos.startPos.position < input.current.startPos.position)
                    {
                        maxPos = input.current;
                        maxLine = pe;
                    }
                }
            }

            if (maxLine != null && maxPos.startPos.position > orig.startPos.position)
            {
                input.current = maxPos;
                return true;
            }

            return false;
        }

        private bool ParseSerial(TTInputTextFile input, TextPosition orig)
        {
            TextPosition linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            foreach (TTPatternEntry pe in Lines)
            {
                pe.Current = 0;
                while (pe.Current < pe.Max)
                {
                    linePos = input.next;
                    bool b = ParseLine(pe, input);
                    if (b)
                    {
                        pe.Current++;
                    }
                    else
                    {
                        input.next = linePos;
                        break;
                    }
                }
                if (pe.Current < pe.Min)
                {
                    input.next = orig;
                    return false;
                }
                index++;
            }

            return true;

        }

        private bool ParseSerial(TTAtomList input, TTAtom orig)
        {
            TTAtom linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            foreach (TTPatternEntry pe in Lines)
            {
                pe.Current = 0;
                while (pe.Current < pe.Max)
                {
                    linePos = input.current;
                    bool b = ParseLine(pe, input);
                    if (b)
                    {
                        pe.Current++;
                    }
                    else
                    {
                        input.current = linePos;
                        break;
                    }
                }
                if (pe.Current < pe.Min)
                {
                    input.current = orig;
                    return false;
                }
                index++;
            }

            return true;

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

        private bool ParseLine(TTPatternEntry pe, TTInputTextFile input)
        {
            if (pe.EntryObject is char)
            {
                CharEntry ce = input.getChar();
                if (ce.eof)
                    return false;
                char c = (char)pe.EntryObject;
                Debugger.Log(0, "", "  ParseLine char " + c + "\n");
                return (c == ce.c);
            }
            else if (pe.EntryObject is string)
            {
                int i = 0;
                string s = pe.EntryObject as string;
                Debugger.Log(0, "", "  ParseLine string " + s + "\n");
                CharEntry ce;
                while (i < s.Length)
                {
                    ce = input.getChar();
                    if (ce.eof)
                        return false;
                    if (ce.c != s[i])
                    {
                        return false;
                    }
                    else
                    {
                        i++;
                    }
                }
                return true;
            }
            else if (pe.EntryObject is TTCharset)
            {
                TTCharset cset = pe.EntryObject as TTCharset;
                Debugger.Log(0, "", "  ParseLine charset \"" + cset.Name + "\"\n");
                CharEntry ce = input.getChar();
                if (ce.eof)
                    return false;
                return (cset.ContainsChar(ce.c));
            }
            else if (pe.EntryObject is TTPattern)
            {
                TTPattern pat = pe.EntryObject as TTPattern;
                Debugger.Log(0, "", "  ParseLine pattern \"" + pat.Name + "\" \n");
                TTAtom tn = pat.ParseAtom(input);
                return (tn != null);
            }

            return false;
        }

        private bool ParseLine(TTPatternEntry pe, TTAtomList input)
        {
            if (pe.EntryObject is TTParserAtom)
            {
                TTAtom ce = input.getAtom();
                if (ce == null)
                    return false;
                TTParserAtom c = (TTParserAtom)pe.EntryObject;
                return c.Match(ce);
            }
            return false;
        }



        public void addAtom(int min, int max, string t, string v)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = new TTParserAtom(t, v);
            Lines.Add(pe);
        }
    }
}
