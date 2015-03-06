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
            /// If Item name == null
            /// then items parsed by this line are not added to tree results
            /// 
            /// if item name == ""
            /// then we will use only subitems of returned tree node
            /// if node has no subitems, we will use returned node itself
            /// 
            /// if item name != ""
            /// then we will use returned node itself
            /// 
            /// </summary>
            public string OutputIdentity = "";

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
        public string OutputIdentity = string.Empty;
        protected List<TTPatternEntry> Lines = new List<TTPatternEntry>();
        public int matchingMethod = METHOD_ALL_SERIAL;

        public const int METHOD_ALL_SERIAL = 0;
        public const int METHOD_FIRST = 1;
        public const int METHOD_MAX = 2;

        public TTPattern()
        {
        }

        public TTPattern(string str)
        {
            Name = str;
        }

        public bool IsParallel
        {
            get { return matchingMethod == METHOD_MAX; }
            set { matchingMethod = (value ? METHOD_MAX : METHOD_ALL_SERIAL); }
        }

        public int MatchingMethod
        {
            get { return matchingMethod; }
            set { matchingMethod = value; }
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

        public void addPattern(int min, int max, TTPattern pat, string itemName)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            pe.EntryObject = pat;
            pe.OutputIdentity = itemName;
            Lines.Add(pe);
        }


        public TTAtom ParseText(TTInputTextFile input)
        {
            TextPosition orig = input.next;
            bool b;

            switch (matchingMethod)
            {
                case METHOD_ALL_SERIAL:
                    b = ParseTextAllSerial(input, orig);
                    break;
                case METHOD_FIRST:
                    b = ParseTextFirst(input, orig);
                    break;
                case METHOD_MAX:
                    b = ParseTextMax(input, orig);
                    break;
                default:
                    b = false;
                    break;
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

        /// <summary>
        /// Main function for parsing atom list into tree
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public TTTreeNode ParseAtoms(TTAtomList input)
        {
            TTAtom orig = input.current;
            TTTreeNode b = null;

            switch (matchingMethod)
            {
                case METHOD_ALL_SERIAL:
                    TTErrorLog.Shared.enterDir("#serial", orig.startPos);
                    b = ParseAtomsAllSerial(input, orig);
                    TTErrorLog.Shared.goUp();
                    break;
                case METHOD_FIRST:
                    TTErrorLog.Shared.enterDir("#first", orig.startPos);
                    b = ParseAtomsFirst(input, orig);
                    TTErrorLog.Shared.goUp();
                    break;
                case METHOD_MAX:
                    TTErrorLog.Shared.enterDir("#paralell", orig.startPos);
                    b = ParseAtomsMax(input, orig);
                    TTErrorLog.Shared.goUp();
                    break;
                default:
                    break;
            }

            if (b != null)
            {
                RationalizeWhenExpression(b);
            }

            return b;
        }

        public bool RationalizeWhenExpression(TTTreeNode node)
        {
            bool b = false;

            if (node.Name.Equals("expression"))
            {
                if (!b) b = TryBinaryDivide(node, null, "oper", "<", "oper.lt");
                if (!b) b = TryBinaryDivide(node, null, "oper", "&", "oper.band");
                if (!b) b = TryBinaryDivide(node, null, "oper", "|", "oper.bor");
                if (!b) b = TryBinaryDivide(node, null, "oper", ">", "oper.gt");
                if (!b) b = TryBinaryDivide(node, null, "oper", "+", "oper.plus");
                if (!b) b = TryBinaryDivide(node, null, "oper", "-", "oper.minus");
                if (!b) b = TryBinaryDivide(node, null, "oper", "*", "oper.mult");
                if (!b) b = TryBinaryDivide(node, null, "oper", "/", "oper.div");
                if (!b) b = TryBinaryDivide(node, null, "oper", "~", "oper.xor");
                if (!b) b = TryBinaryDivide(node, null, "oper", "%", "oper.mod");
                if (!b) b = TryBinaryDivide(node, null, "oper", "<<", "oper.lshift");
                if (!b) b = TryBinaryDivide(node, null, "oper", ">>", "oper.rshift");
                if (!b) b = TryBinaryDivide(node, null, "oper", "&&", "oper.land");
                if (!b) b = TryBinaryDivide(node, null, "oper", "||", "oper.lor");
                if (!b) b = TryBinaryDivide(node, null, "oper", "<=", "oper.le");
                if (!b) b = TryBinaryDivide(node, null, "oper", ">=", "oper.ge");
                if (!b) b = TryBinaryDivide(node, null, "oper", "!=", "oper.neq");
                if (!b) b = TryBinaryDivide(node, null, "oper", "==", "oper.eq");
                if (!b) b = TryBinaryDivide(node, null, "oper", "=", "oper.assign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "&=", "oper.bandassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "|=", "oper.borassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "+=", "oper.plusassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "-=", "oper.minusassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "*=", "oper.multassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "/=", "oper.divassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "~=", "oper.xorassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "%=", "oper.modassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "<<=", "oper.lshiftassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", ">>=", "oper.rshiftassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "&&=", "oper.landassign");
                if (!b) b = TryBinaryDivide(node, null, "oper", "||=", "oper.lorassign");
                /*if (!b) b = TryUnaryPrefix(node, null, "oper", "++", "unary.inc.prefix", "expression");
                if (!b) b = TryUnaryPostfix(node, null, "oper", "++", "unary.inc.postfix", "expression");
                */

                if (b)
                {
                    foreach (TTTreeNode iter in node.Children)
                    {
                        RationalizeWhenExpression(iter);
                    }
                }
            }

            return true;
        }

        public bool TryUnaryPrefix(TTTreeNode node, string nodeName, string atomType, string atomValue, string newNodeName, string newSubnodeName)
        {
            TTTreeNode tn;
            tn = node.getHead();
            if (tn != null)
            {
                if (tn.canMatchAtom(nodeName, atomType, atomValue))
                {
                    TTTreeNodeCollection subs = node.takeChildrenAfter(tn);
                    tn.addCollection(subs);
                    tn.Name = newSubnodeName;
                    node.Name = newNodeName;
                    return true;
                }
            }

            return false;
        }

        public bool TryUnaryPostfix(TTTreeNode node, string nodeName, string atomType, string atomValue, string newNodeName, string newSubnodeName)
        {
            TTTreeNode tn;
            tn = node.getTail();
            if (tn != null)
            {
                if (tn.canMatchAtom(nodeName, atomType, atomValue))
                {
                    TTTreeNodeCollection subs = node.takeChildrenBefore(tn);
                    tn.addCollection(subs);
                    tn.Name = newSubnodeName;
                    node.Name = newNodeName;
                    return true;
                }
            }

            return false;
        }

        public bool TryBinaryDivide(TTTreeNode node, string nodeName, string atomType, string atomValue, string newNodeName)
        {
            TTTreeNode dividerNode;
            dividerNode = node.findNodeForward(nodeName, atomType, atomValue, null);
            if (dividerNode != null)
            {
                node.atom = null;

                TTTreeNodeCollection a1, a2;

                a1 = node.takeChildrenBefore(dividerNode);
                node.removeHead();
                a2 = node.takeAllChildren();

                node.addSubnode(a1.createEncapsulationNode("expression"));
                node.addSubnode(a2.createEncapsulationNode("expression"));
                node.Name = newNodeName;

                return true;
            }
            else
            {
                return false;
            }
        }

        private TTTreeNode ParseAtomsMax(TTAtomList input, TTAtom orig)
        {
            TTAtom maxPos = orig;
            TTTreeNode maxTree = null;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.current = orig;
                TTTreeNode currTree = ParseLineAtoms(pe, input);
                if (currTree != null)
                {
                    //if (pe.OutputIdentity != null && pe.OutputIdentity.Length > 0 && (currTree.Name == null || currTree.Name.Length == 0))
                        currTree.Name = pe.OutputIdentity;
                    // this is like negation
                    // if this line was hit, then stop using this parser
                    if (pe.Max < 0)
                    {
                        input.current = orig;
                        return null;
                    }
                    if (maxPos.startPos.position < input.current.startPos.position)
                    {
                        maxPos = input.current;
                        maxLine = pe;
                        maxTree = currTree;
                    }
                }
            }

            if (maxLine != null && maxPos.startPos.position > orig.startPos.position)
            {
                input.current = maxPos;
                return maxTree;
            }

            return null;
        }

        private TTTreeNode ParseAtomsFirst(TTAtomList input, TTAtom orig)
        {
            foreach (TTPatternEntry pe in Lines)
            {
                input.current = orig;
                TTTreeNode tn = ParseLineAtoms(pe, input);
                if (tn != null && pe.OutputIdentity != null && pe.OutputIdentity.Length > 0 && (tn.Name != null && tn.Name.Length == 0))
                    tn.Name = pe.OutputIdentity;
                if (tn != null)
                {
                    return tn;
                }
            }

            input.current = orig;
            return null;
        }

        private bool ParseTextFirst(TTInputTextFile input, TextPosition orig)
        {
            foreach (TTPatternEntry pe in Lines)
            {
                input.next = orig;
                if (ParseLineText(pe, input))
                {
                    return true;
                }
            }

            input.next = orig;
            return false;
        }

        private bool ParseTextMax(TTInputTextFile input, TextPosition orig)
        {
            TextPosition maxPos = orig;
            TTPatternEntry maxLine = null;

            foreach (TTPatternEntry pe in Lines)
            {
                input.next = orig;
                if (ParseLineText(pe, input))
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

        private bool ParseTextAllSerial(TTInputTextFile input, TextPosition orig)
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
                    bool b = ParseLineText(pe, input);
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

        private TTTreeNode ParseAtomsAllSerial(TTAtomList input, TTAtom orig)
        {
            TTAtom linePos;
            int index = 0;
            int maxIndex = Lines.Count - 1;

            TTTreeNode rv = new TTTreeNode();
            rv.Name = this.OutputIdentity;

            foreach (TTPatternEntry pe in Lines)
            {
                pe.Current = 0;
                while (pe.Current < pe.Max)
                {
                    linePos = input.current;
                    TTTreeNode lv = ParseLineAtoms(pe, input);
                    if (lv != null)
                    {
                        if (pe.OutputIdentity != null && pe.OutputIdentity.Length > 0 && lv.Name.Length == 0)
                            lv.Name = pe.OutputIdentity;
                        if (lv.Name == null)
                        {
                        }
                        else if (lv.Name.Length == 0 && lv.atom == null)
                        {
                            rv.addCollection(lv.Children);
                        }
                        else
                        {
                            rv.addSubnode(lv);
                        }
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
                    return null;
                }
                index++;
            }

            return rv;

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

        private bool ParseLineText(TTPatternEntry pe, TTInputTextFile input)
        {
            if (pe.EntryObject is char)
            {
                CharEntry ce = input.getChar();
                if (ce.eof)
                    return false;
                char c = (char)pe.EntryObject;
                bool b = (c == ce.c);
                TTErrorLog.Shared.addDir("char " + c, b, ce.pos);
                return b;
            }
            else if (pe.EntryObject is string)
            {
                int i = 0;
                string s = pe.EntryObject as string;
                CharEntry ce;
                while (i < s.Length)
                {
                    ce = input.getChar();
                    if (ce.eof)
                    {
                        TTErrorLog.Shared.addDir("string " + s, false, ce.pos);
                        return false;
                    }
                    if (ce.c != s[i])
                    {
                        TTErrorLog.Shared.addDir("string " + s, false, ce.pos);
                        return false;
                    }
                    else
                    {
                        i++;
                    }
                }
                TTErrorLog.Shared.addDir("string " + s, true, input.next);
                return true;
            }
            else if (pe.EntryObject is TTCharset)
            {
                TTCharset cset = pe.EntryObject as TTCharset;
                CharEntry ce = input.getChar();
                if (ce.eof)
                {
                    TTErrorLog.Shared.addDir("charset " + cset.Name, false, ce.pos);
                    return false;
                }
                bool b = (cset.ContainsChar(ce.c));
                TTErrorLog.Shared.addDir("charset " + cset.Name, b, ce.pos);
                return b;
            }
            else if (pe.EntryObject is TTPattern)
            {
                TTPattern pat = pe.EntryObject as TTPattern;
                TTErrorLog.Shared.enterDir("pattern " + pat.Name, input.next);
                TTAtom tn = pat.ParseText(input);
                TTErrorLog.Shared.goUp();
                return (tn != null);
            }

            TTErrorLog.Shared.addDir("unknown line", false, input.next);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pe"></param>
        /// <param name="input"></param>
        /// <returns>Returns tree node which contains:
        /// If parser object is atom, then tree node refers directly to that atom
        /// If parser object is pattern, then item parsed by pattern are child items of returned tree node
        /// Returned tree node has name given by value of ItemName of pattern line entry.
        /// </returns>
        private TTTreeNode ParseLineAtoms(TTPatternEntry pe, TTAtomList input)
        {
            if (pe.EntryObject is TTParserAtom)
            {
                TTAtom ce = input.getAtom();
                if (ce == null)
                    return null;
                TTParserAtom c = (TTParserAtom)pe.EntryObject;
                if (c.Match(ce))
                {
                    TTErrorLog.Shared.addDir("atom " + c.ToString(), true, ce.startPos);
                    TTTreeNode rv = new TTTreeNode();
                    rv.Name = c.OutputIdentity;
                    if (pe.OutputIdentity == null)
                    {
                        rv.Name = null;
                    }
                    rv.atom = ce;
                    return rv;
                }
                else
                {
                    TTErrorLog.Shared.addDir("atom " + c.ToString(), false, ce.startPos);
                    return null;
                }
            }
            else if (pe.EntryObject is TTPattern)
            {
                TTPattern pat = pe.EntryObject as TTPattern;
                TTErrorLog.Shared.enterDir("pattern " + pat.Name, input.current.startPos);
                TTTreeNode rv = pat.ParseAtoms(input);
                if (rv != null && (rv.Name != null && rv.Name.Length == 0)
                    && pat.OutputIdentity != null && pat.OutputIdentity.Length > 0)
                {

                    rv.Name = pat.OutputIdentity;
                }
                TTErrorLog.Shared.goUp();
                return rv;
            }

            TTErrorLog.Shared.addDir("unknown line", false, input.current.startPos);
            return null;
        }



        public void addAtom(int min, int max, string t, string v, string itemName)
        {
            TTPatternEntry pe = new TTPatternEntry();
            pe.Min = min;
            pe.Max = max;
            TTParserAtom pa = new TTParserAtom();
            pa.TestType = t;
            pa.TestValue = v;
            pa.OutputIdentity = itemName;
            pe.EntryObject = pa;
            pe.OutputIdentity = itemName;
            Lines.Add(pe);
        }
    }
}
